using MediCorePatientFlow.Data;
using MediCorePatientFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace MediCorePatientFlow.Services
{
    public class GcnService
    {
        private readonly ApplicationDbContext _db;

        public GcnService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Calculate GCN from a list of registration IDs (strings)
        public int CalculateGcn(IEnumerable<string> registrationIds)
        {
            var sum = 0;
            foreach (var id in registrationIds)
            {
                foreach (var ch in id)
                {
                    if (char.IsDigit(ch)) sum += ch - '0';
                }
            }
            return sum % 5;
        }

        // Example seeding influence: documented in code
        public async Task SeedInfluencedDataAsync(IEnumerable<string> registrationIds)
        {
            var gcn = CalculateGcn(registrationIds);

            // Influence example:
            // - Last two digits of first reg determine number of patients to seed (mod 20)
            // - Middle two digits determine extra beds per ward (0-3)
            // - First two digits determine default doctor availability pattern

            var first = registrationIds.FirstOrDefault() ?? "00";
            int lastTwo = 0;
            int middleTwo = 0;
            int firstTwo = 0;
            var digits = new string(first.Where(char.IsDigit).ToArray());
            if (digits.Length >= 2) firstTwo = int.Parse(digits.Substring(0, 2));
            if (digits.Length >= 4) middleTwo = int.Parse(digits.Substring(digits.Length / 2 - 1, 2));
            if (digits.Length >= 2) lastTwo = int.Parse(digits.Substring(digits.Length - 2, 2));

            int patientsToSeed = lastTwo % 20;
            int extraBeds = middleTwo % 4;
            int availabilitySeed = firstTwo % 3; // 0: Available,1:OnDuty,2:OnLeave

            // Seed doctors
            if (!await _db.Doctors.AnyAsync())
            {
                var wards = new[] { "General", "ICU", "Maternity", "Pediatric" };
                for (int i = 1; i <= 8; i++)
                {
                    _db.Doctors.Add(new Doctor
                    {
                        FirstName = $"Doc{i}",
                        LastName = "Auto",
                        Specialization = i % 2 == 0 ? "General" : "Specialist",
                        AssignedWard = wards[i % wards.Length],
                        AvailabilityStatus = availabilitySeed == 0 ? "Available" : (availabilitySeed == 1 ? "OnDuty" : "OnLeave"),
                        ConsultationScheduleJson = "[]"
                    });
                }
            }

            // Seed patients
            if (!await _db.Patients.AnyAsync())
            {
                for (int i = 1; i <= patientsToSeed; i++)
                {
                    _db.Patients.Add(new Patient
                    {
                        FirstName = $"Seed{i}",
                        LastName = "Patient",
                        DateOfBirth = DateTime.Today.AddYears(-30).AddDays(i),
                        Gender = i % 2 == 0 ? "Female" : "Male",
                        ContactNumber = null
                    });
                }
            }

            await _db.SaveChangesAsync();
        }
    }
}
