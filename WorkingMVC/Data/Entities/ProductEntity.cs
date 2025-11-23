using System.Collections.Generic; // Для ICollection
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkingMVC.Data.Entities;

[Table("tblProducts")]
public class ProductEntity : BaseEntity<int>
{
    // ------------------ ВЛАСТИВОСТІ ТОВАРУ ------------------

    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    // ------------------ ЗВ'ЯЗОК З КАТЕГОРІЄЮ (Foreign Key) ------------------

    [ForeignKey(nameof(Category))]
    public int CategoryId { get; set; }

    // Навігаційна властивість: Обов'язковий зв'язок 
    public CategoryEntity Category { get; set; } = null!;

    // ------------------ ЗВ'ЯЗКИ "ОДИН ДО БАГАТЬОХ" (Ініціалізація) ------------------

    // Зв'язок з фотографіями: Один товар - багато фотографій
    public ICollection<ProductImageEntity> ProductImages { get; set; } = new List<ProductImageEntity>();

    // Зв'язок з кошиком: Багато-до-багатьох (якщо Carts це проміжна таблиця)
    public ICollection<CartEntity> Carts { get; set; } = new List<CartEntity>();

    // Зв'язок з елементами замовлення: Один товар може бути в багатьох елементах замовлення
    public ICollection<OrderItemEntity> OrderItems { get; set; } = new List<OrderItemEntity>();
}