using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WEB_6.Models;
using WEB_6.Data;
using Microsoft.EntityFrameworkCore;

namespace WEB_6.Controllers;   

[Authorize(Roles = "Admin")]
public class VehiclesController : Controller
{
    private readonly ApplicationDbContext _context;

    public VehiclesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Vehicles - List all vehicles sorted by Id descending
    public async Task<IActionResult> Index()
    {
        var vehicles = _context.Vehicles.Include(v => v.VehicleType)
                                        .Include(v => v.VehicleImages)
                                        .OrderByDescending(v => v.Id);
        return View(await vehicles.ToListAsync());
    }

    // GET: Vehicles/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicle = await _context.Vehicles
            .Include(v => v.VehicleType)
            .Include(v => v.VehicleImages)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (vehicle == null)
        {
            return NotFound();
        }

        return View(vehicle);
    }

    // GET: Vehicles/Create
    [HttpGet]
    public IActionResult Create()
    {
        ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes, "Id", "Name");
        return View();
    }

    // POST: Vehicles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Vehicle vehicle)
    {
        if (ModelState.IsValid)
        {
            vehicle.TrangThai = true;
            _context.Add(vehicle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes, "Id", "Name", vehicle.VehicleTypeId);
        return View(vehicle);
    }

    // GET: Vehicles/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null)
        {
            return NotFound();
        }

        ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes, "Id", "Name", vehicle.VehicleTypeId);
        return View(vehicle);
    }

    // POST: Vehicles/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Vehicle vehicle)
    {
        if (id != vehicle.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(vehicle);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehicleExists(vehicle.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes, "Id", "Name", vehicle.VehicleTypeId);
        return View(vehicle);
    }

    // GET: Vehicles/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicle = await _context.Vehicles
            .Include(v => v.VehicleType)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (vehicle == null)
        {
            return NotFound();
        }

        return View(vehicle);
    }

    // POST: Vehicles/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle != null)
        {
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // GET: Vehicles/TraCuu - Search vehicle by BienSoXe
    [AllowAnonymous]
    public async Task<IActionResult> TraCuu(string SearchString)
    {
        if (string.IsNullOrEmpty(SearchString))
        {
            return View();
        }

        var vehicles = await _context.Vehicles.Include(v => v.VehicleType)
                                        .Include(v => v.VehicleImages)
                                        .FirstOrDefaultAsync(v => v.BienSoXe == SearchString && v.TrangThai == true);

        if (vehicles == null)
        {
            ViewBag.Message = "Không tìm thấy phương tiện nào với biển số xe đã nhập.";
        }
        return View(vehicles);
    }

    private bool VehicleExists(int id)
    {
        return _context.Vehicles.Any(e => e.Id == id);
    }
}