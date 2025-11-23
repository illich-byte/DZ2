using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkingMVC.Data.Entities;

[Table("tblProductImages")]
public class ProductImageEntity : BaseEntity<int>
{
    // ------------------ КОРЕКЦІЯ ------------------
    // Замість Name використовуємо ImageUrl, який повертає IImageService
    [Required] // URL зображення обов'язковий
    [StringLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    // Додаткове поле для опису або імені файлу
    [StringLength(255)]
    public string ImageDescription { get; set; } = string.Empty;

    // Замість Priority використовуємо IsMain, щоб визначити головне фото
    public bool IsMain { get; set; } // Використовується для сортування та відображення на Front-end

    // ------------------ ЗВ'ЯЗОК ------------------
    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }

    public ProductEntity? Product { get; set; }
}