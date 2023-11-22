using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectroMVC.Models
{
	public class Subscribe
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
		public string? Email { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}

