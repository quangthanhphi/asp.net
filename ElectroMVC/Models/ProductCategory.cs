using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectroMVC.Models
{
	
    public class ProductCategory : CommonAbstract
    {
        public ProductCategory()
        {
            this.Products = new HashSet<Product>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string? Title { get; set; }
        public string? Alias { get; set; }
        [Display(Name = "Ảnh đại diện")]
        [DataType(DataType.Upload)]
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? SeoTitle { get; set; }
        public string? SeoDescription { get; set; }
        public string? SeoKeywords { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}

