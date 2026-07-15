using Microsoft.AspNetCore.Mvc;
using MediCorePatientFlow.Services;
using MediCorePatientFlow.Data;
using Microsoft.EntityFrameworkCore;

namespace MediCorePatientFlow.Controllers
{
    public class HomeController : Controller
    {
        private readonly GcnService _gcn;
        private readonly BedAllocationService _beds;
        private readonly ApplicationDbContext _db;

        public HomeController(GcnService gcn, BedAllocationService beds, ApplicationDbContext db)
        {
            _gcn = gcn;
            _beds = beds;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // For demo, compute GCN from example registration ids
            var regs = new[] { "REG2026001", "REG2026002" };
            var gcnValue = _gcn.CalculateGcn(regs);

            var model = new
            {
                Gcn = gcnValue,
                TotalPatients = await _db.Patients.CountAsync(),
                TotalAdmissions = await _db.Admissions.CountAsync()
            };

            return View(model);
        }
    }
}
