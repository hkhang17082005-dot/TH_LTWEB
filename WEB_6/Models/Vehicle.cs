using System.ComponentModel.DataAnnotations;

namespace WEB_6.Models;

public class VehicleType
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Vui lòng nhập tên loại phương tiện")]
    [Display(Name = "Loại phương tiện")]
    public string Name { get; set; } = string.Empty;
    
    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}

public class Vehicle
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Biển số xe là bắt buộc nhập")]
    [RegularExpression(@"^[0-9]{2}[a-zA-Z][0-9]-[0-9]{5}$", ErrorMessage = "Định dạng không hợp lệ (VD: 54P6-13844)")]
    [Display(Name = "Biển số xe")]
    public string BienSoXe { get; set; } = string.Empty;

    [Display(Name = "Thời gian vi phạm")]
    public DateTime ThoiGianViPham { get; set; }

    [DataType(DataType.MultilineText)]
    [Display(Name = "Nội dung")]
    public string NoiDung { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn loại phương tiện")]
    [Display(Name = "Loại phương tiện")]
    public int VehicleTypeId { get; set; }
    
    public VehicleType VehicleType { get; set; } = null!;

    [Display(Name = "Hình ảnh vi phạm")]
    public string? HinhAnh { get; set; }

    public bool TrangThai { get; set; } = false;
    
    // Collection nhiều hình ảnh
    public ICollection<VehicleImage> VehicleImages { get; set; } = new List<VehicleImage>();
}
