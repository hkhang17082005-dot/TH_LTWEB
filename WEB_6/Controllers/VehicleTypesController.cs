using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WEB_6.Data;
using WEB_6.Models;
using Microsoft.EntityFrameworkCore;

namespace WEB_6.Controllers;

[Authorize(Roles = "Admin")]
public class VehicleTypesController : Controller
{
    private readonly ApplicationDbContext _context;

    public VehicleTypesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: VehicleTypes
    public async Task<IActionResult> Index()
    {
        return View(await _context.VehicleTypes.ToListAsync());
    }

    // GET: VehicleTypes/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicleType = await _context.VehicleTypes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (vehicleType == null)
        {
            return NotFound();
        }

        return View(vehicleType);
    }

    // GET: VehicleTypes/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: VehicleTypes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VehicleType vehicleType)
    {
        if (ModelState.IsValid)
        {
            _context.Add(vehicleType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(vehicleType);
    }

    // GET: VehicleTypes/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicleType = await _context.VehicleTypes.FindAsync(id);
        if (vehicleType == null)
        {
            return NotFound();
        }
        return View(vehicleType);
    }

    // POST: VehicleTypes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VehicleType vehicleType)
    {
        if (id != vehicleType.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(vehicleType);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehicleTypeExists(vehicleType.Id))
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
        return View(vehicleType);
    }

    // GET: VehicleTypes/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicleType = await _context.VehicleTypes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (vehicleType == null)
        {
            return NotFound();
        }

        return View(vehicleType);
    }

    // POST: VehicleTypes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var vehicleType = await _context.VehicleTypes.FindAsync(id);
        if (vehicleType != null)
        {
            _context.VehicleTypes.Remove(vehicleType);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool VehicleTypeExists(int id)
    {
        return _context.VehicleTypes.Any(e => e.Id == id);
    }
}
