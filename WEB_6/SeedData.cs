using Microsoft.AspNetCore.Identity;
using WEB_6.Data;
using WEB_6.Models;

namespace WEB_6
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // Tạo database
                await context.Database.EnsureCreatedAsync();

                // Tạo các Roles
                string[] roles = { "Admin", "User" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // Tạo tài khoản Admin
                var adminEmail = "admin@example.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FullName = "Quản trị viên",
                        EmailConfirmed = true,
                        IsActive = true
                    };
                    var result = await userManager.CreateAsync(adminUser, "Admin@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }

                // Tạo tài khoản User mẫu
                var userEmail = "user@example.com";
                var normalUser = await userManager.FindByEmailAsync(userEmail);
                if (normalUser == null)
                {
                    normalUser = new ApplicationUser
                    {
                        UserName = userEmail,
                        Email = userEmail,
                        FullName = "Người dùng",
                        EmailConfirmed = true,
                        IsActive = true
                    };
                    var result = await userManager.CreateAsync(normalUser, "User@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(normalUser, "User");
                    }
                }

                // Tạo VehicleTypes
                if (!context.VehicleTypes.Any())
                {
                    var vehicleTypes = new VehicleType[]
                    {
                        new VehicleType { Name = "Xe máy" },
                        new VehicleType { Name = "Ô tô 4 chỗ" },
                        new VehicleType { Name = "Ô tô tải" },
                        new VehicleType { Name = "Xe buýt" },
                        new VehicleType { Name = "Xe tải" },
                        new VehicleType { Name = "Xe đạp" }
                    };
                    context.VehicleTypes.AddRange(vehicleTypes);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
