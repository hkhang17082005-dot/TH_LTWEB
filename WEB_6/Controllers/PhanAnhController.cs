using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WEB_6.Models;
using WEB_6.Data;
using Microsoft.EntityFrameworkCore;

namespace WEB_6.Controllers;   

[Authorize(Roles = "User")]
public class PhanAnhController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public PhanAnhController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes, "Id", "Name");
        return View();
    }

    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Vehicle vehicle, IFormFileCollection HinhAnhUpload)
    {

        ModelState.Remove("VehicleType"); // Loại bỏ validation cho trường TrangThai vì nó sẽ được set mặc định trong controller
        if (ModelState.IsValid)
        {
            vehicle.TrangThai = true; // Mặc định là True khi user gửi
            _context.Add(vehicle);
            await _context.SaveChangesAsync();

            // Xử lý upload nhiều hình ảnh
            if (HinhAnhUpload != null && HinhAnhUpload.Count > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                
                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                foreach (var file in HinhAnhUpload)
                {
                    if (file.Length > 0)
                    {
                        // Tạo tên file unique
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string filePath = Path.Combine(uploadsFolder, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        // Lưu thông tin hình ảnh vào database
                        var vehicleImage = new VehicleImage
                        {
                            VehicleId = vehicle.Id,
                            ImagePath = fileName
                        };
                        _context.VehicleImages.Add(vehicleImage);
                    }
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("TraCuu", "Vehicles");
        }
        ViewData["VehicleTypeId"] = new SelectList(_context.VehicleTypes, "Id", "Name", vehicle.VehicleTypeId);
        return View(vehicle);
    }
}