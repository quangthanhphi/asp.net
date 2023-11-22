using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectroMVC.Models
{
	public class Adv : CommonAbstract
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string? Title { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(500)]
        public string? Image { get; set; }
        [StringLength(500)]
        public string? Link { get; set; }
        public int Type { get; set; }
    }
}

