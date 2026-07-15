using Microsoft.AspNetCore.Mvc;
using MediCorePatientFlow.Data;
using MediCorePatientFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace MediCorePatientFlow.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DepartmentsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Receptionist,Doctor,Nurse")]
        public async Task<IActionResult> Index()
        {
            var list = await _db.Departments.ToListAsync();
            return View(list);
        }

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Receptionist")]
        public IActionResult Create() => View();

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Receptionist")]
        public async Task<IActionResult> Create(Department dept)
        {
            if (!ModelState.IsValid) return View(dept);
            _db.Departments.Add(dept);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
