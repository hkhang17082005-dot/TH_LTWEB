using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WEB_6.Models;

namespace WEB_6.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        [TempData]
        public string ErrorMessage { get; set; } = default!;

        public class InputModel
        {
            [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Họ tên là bắt buộc")]
            [System.ComponentModel.DataAnnotations.Display(Name = "Họ tên")]
            public string FullName { get; set; } = string.Empty;

            [System.ComponentModel.DataAnnotations.EmailAddress]
            [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Email là bắt buộc")]
            public string Email { get; set; } = string.Empty;

            [System.ComponentModel.DataAnnotations.StringLength(100, ErrorMessage = "Mật khẩu phải dài từ {2} đến {1} ký tự.", MinimumLength = 6)]
            [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
            [System.ComponentModel.DataAnnotations.Display(Name = "Mật khẩu")]
            [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Mật khẩu là bắt buộc")]
            public string Password { get; set; } = string.Empty;

            [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
            [System.ComponentModel.DataAnnotations.Display(Name = "Xác nhận mật khẩu")]
            [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FullName = Input.FullName,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Gán role mặc định "User" cho người dùng mới
                    if (!await _roleManager.RoleExistsAsync("User"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("User"));
                    }

                    await _userManager.AddToRoleAsync(user, "User");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(Url.Content("~/"));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}
