using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectroMVC.Models
{
	public class Order : CommonAbstract
	{
        public Order()
        {
            this.orderDetails = new HashSet<OrderDetail>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string? Code { get; set; }
        [Required]
        public string? CustomerName { get; set; }
        [Required]
        public string? Phone { get; set; }
        [Required]
        public decimal Address { get; set; }
        public int Quantity { get; set; }
        public ICollection<OrderDetail> orderDetails { get; set; }
    }
}

