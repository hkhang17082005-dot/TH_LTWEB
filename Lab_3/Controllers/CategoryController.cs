using Lab_3.Models;
using Lab_3.Respositories;
using Microsoft.AspNetCore.Mvc;

namespace Lab_3.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRespository _categoryRespository;

        public CategoryController(ICategoryRespository categoryRespository)
        {
            _categoryRespository = categoryRespository;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRespository.GetAllAsync();
            return View(categories);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRespository.AddAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Update(int id)
        {
            var category = await _categoryRespository.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (id != category.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _categoryRespository.UpdateAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRespository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryRespository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}