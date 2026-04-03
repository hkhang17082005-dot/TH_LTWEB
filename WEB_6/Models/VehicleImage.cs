using System.ComponentModel.DataAnnotations;

namespace WEB_6.Models;

public class VehicleImage
{
    public int Id { get; set; }
    
    [Required]
    public int VehicleId { get; set; }
    
    public Vehicle Vehicle { get; set; } = null!;
    
    [Required]
    [Display(Name = "Hình ảnh")]
    public string ImagePath { get; set; } = string.Empty;
    
    [Display(Name = "Ngày tải lên")]
    public DateTime UploadedDate { get; set; } = DateTime.Now;
}
