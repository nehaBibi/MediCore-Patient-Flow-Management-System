using Microsoft.AspNetCore.Mvc;
using MediCorePatientFlow.Data;
using MediCorePatientFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace MediCorePatientFlow.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PatientsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Receptionist,Nurse,Doctor")]
        public async Task<IActionResult> Index()
        {
            var list = await _db.Patients.ToListAsync();
            return View(list);
        }

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Receptionist")]
        public IActionResult Create() => View();

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Receptionist")]
        public async Task<IActionResult> Create(Patient patient)
        {
            if (!ModelState.IsValid) return View(patient);
            _db.Patients.Add(patient);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Receptionist")]
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _db.Patients.FindAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Receptionist")]
        public async Task<IActionResult> Edit(Patient patient)
        {
            if (!ModelState.IsValid) return View(patient);
            _db.Patients.Update(patient);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Receptionist")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Patients.FindAsync(id);
            if (p == null) return NotFound();
            _db.Patients.Remove(p);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
