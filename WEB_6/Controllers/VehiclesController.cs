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
    private readonly IWebHostEnvironment _webHostEnvironment;
    public VehiclesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
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
    public async Task<IActionResult> Create(Vehicle vehicle, IFormFile HinhAnhUpload)
    {
        if (ModelState.IsValid)
        {
            //XỬ LÍ UPLOAD HÌNH ẢNH
            if (HinhAnhUpload != null && HinhAnhUpload.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                
                // Nếu thư mục chưa có thì tạo mới
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Tạo tên file ngẫu nhiên để không bị trùng lặp
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(HinhAnhUpload.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                // Copy file vào server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await HinhAnhUpload.CopyToAsync(fileStream);
                }
                vehicle.HinhAnh = fileName; 
            }

            vehicle.TrangThai = true;
            _context.Add(vehicle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes, "Id", "Name", vehicle.VehicleTypeId);
        return View(vehicle);
    }
    //GET: XỬ LÍ PHẢN ÁNH
    [HttpGet]
    public async Task<IActionResult> XuLyPhanAnh()
    {
        // Lấy danh sách các phương tiện do User gửi lên chưa được duyệt (TrangThai == false)
        var pendingVehicles = _context.Vehicles
            .Include(v => v.VehicleType)
            .Where(v => v.TrangThai == false)
            .OrderByDescending(v => v.ThoiGianViPham);
            
        return View(await pendingVehicles.ToListAsync());
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
        if (id != vehicle.Id) return NotFound();

        ModelState.Remove("VehicleType");

        if (ModelState.IsValid)
        {
            try
            {
                vehicle.TrangThai = true; // Mặc định là True khi Admin duyệt

                _context.Update(vehicle);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehicleExists(vehicle.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(XuLyPhanAnh));
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