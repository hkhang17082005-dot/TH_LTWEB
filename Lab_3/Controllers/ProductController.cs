using Lab_3.Models;
using Lab_3.Respositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab_3.Controllers 
{
    // [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IProductRespository _productRespository;
        private readonly ICategoryRespository _categoryRespository;

        public ProductController(IProductRespository productRespository, ICategoryRespository categoryRespository)
        {
            _productRespository = productRespository;
            _categoryRespository = categoryRespository;
        }

        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var products = await _productRespository.GetAllAsync();
            return View(products);
        }

        // Hiển thị form thêm sản phẩm
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRespository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        // Xử lý thêm sản phẩm
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    product.ImageURL = await SaveImage(imageUrl);
                }

                await _productRespository.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRespository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        // Hiển thị chi tiết sản phẩm
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRespository.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // Hiển thị form cập nhật sản phẩm
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRespository.GetByIdAsync(id);
            if (product == null) return NotFound();

            var categories = await _categoryRespository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // Xử lý cập nhật sản phẩm
        [HttpPost]
        public async Task<IActionResult> Update(int id, Product product, IFormFile imageUrl)
        {
            ModelState.Remove("ImageUrl"); // Loại bỏ kiểm tra validate cho chuỗi URL nếu upload file

            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existingProduct = await _productRespository.GetByIdAsync(id);
                if (existingProduct == null) return NotFound();

                if (imageUrl != null)
                {
                    product.ImageURL = await SaveImage(imageUrl);
                }
                else
                {
                    product.ImageURL = existingProduct.ImageURL;
                }

                // Cập nhật thông tin
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ImageURL = product.ImageURL;

                await _productRespository.UpdateAsync(existingProduct);
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRespository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // Hiển thị form xác nhận xóa
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRespository.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // Xử lý xóa thực sự
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRespository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Helper lưu hình ảnh
        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName;
        }
    }
}