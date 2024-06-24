using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFramework.Entities;

[Table("Categories")]
public class Category
{
    public Category(long categoryId)
    {
        this.CategoryId = categoryId;
    }

    [Key]
    public long CategoryId { get; set; }

    [MaxLength(255)]
    public string CategoryName { get; set; } = default!;

    [MaxLength(255)]
    public string Description { get; set; } = default!;
}
