using Microsoft.AspNetCore.Mvc;
using MediCorePatientFlow.Data;
using MediCorePatientFlow.Models;
using MediCorePatientFlow.Services;
using Microsoft.EntityFrameworkCore;

namespace MediCorePatientFlow.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class AdmissionsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly BedAllocationService _beds;

        public AdmissionsController(ApplicationDbContext db, BedAllocationService beds)
        {
            _db = db;
            _beds = beds;
        }

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Receptionist,Nurse,Doctor")]
        public async Task<IActionResult> Index()
        {
            var list = await _db.Admissions.Include(a => a.Patient).Include(a => a.Doctor).ToListAsync();
            return View(list);
        }

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Receptionist")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Patients = await _db.Patients.ToListAsync();
            ViewBag.Doctors = await _db.Doctors.ToListAsync();
            return View();
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Receptionist")]
        public async Task<IActionResult> Create(Admission admission)
        {
            if (!ModelState.IsValid) return View(admission);

            admission.AdmissionDate = DateTime.UtcNow;
            admission.AdmissionStatus = "Admitted";

            // validate bed
            var success = await _beds.AllocateBedAsync(admission);
            if (!success)
            {
                ModelState.AddModelError("", "Bed already occupied or unavailable.");
                ViewBag.Patients = await _db.Patients.ToListAsync();
                ViewBag.Doctors = await _db.Doctors.ToListAsync();
                return View(admission);
            }

            return RedirectToAction(nameof(Index));
        }

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Nurse,Doctor")]
        public async Task<IActionResult> Discharge(int id)
        {
            var admission = await _db.Admissions.FindAsync(id);
            if (admission == null) return NotFound();
            admission.DischargeDate = DateTime.UtcNow;
            admission.AdmissionStatus = "Discharged";
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
