using MediCorePatientFlow.Data;
using MediCorePatientFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace MediCorePatientFlow.Services
{
    public class BedAllocationService
    {
        private readonly ApplicationDbContext _db;

        // Static ward capacities; can be influenced by seeding logic
        private readonly Dictionary<string, int> _baseWardCapacities = new()
        {
            { "General", 30 },
            { "ICU", 8 },
            { "Maternity", 12 },
            { "Pediatric", 10 }
        };

        public BedAllocationService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<int> GetOccupiedBedsAsync(string ward)
        {
            return await _db.Admissions.CountAsync(a => a.WardName == ward && a.DischargeDate == null);
        }

        public async Task<int> GetAvailableBedsAsync(string ward)
        {
            var occupied = await GetOccupiedBedsAsync(ward);
            var capacity = _baseWardCapacities.ContainsKey(ward) ? _baseWardCapacities[ward] : 0;
            return Math.Max(0, capacity - occupied);
        }

        // Suggest a bed number (string) using a simple LINQ-based algorithm
        public async Task<string?> SuggestBedAsync(string ward, Patient patient)
        {
            var capacity = _baseWardCapacities.ContainsKey(ward) ? _baseWardCapacities[ward] : 0;
            var occupiedBeds = await _db.Admissions
                .Where(a => a.WardName == ward && a.DischargeDate == null && a.BedNumber != null)
                .Select(a => a.BedNumber)
                .ToListAsync();

            for (int i = 1; i <= capacity; i++)
            {
                var candidate = i.ToString();
                if (!occupiedBeds.Contains(candidate)) return candidate;
            }
            return null;
        }

        // Validate no double allocation
        public async Task<bool> AllocateBedAsync(Admission admission)
        {
            var occupied = await _db.Admissions.AnyAsync(a => a.WardName == admission.WardName && a.BedNumber == admission.BedNumber && a.DischargeDate == null);
            if (occupied) return false;

            _db.Admissions.Add(admission);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
