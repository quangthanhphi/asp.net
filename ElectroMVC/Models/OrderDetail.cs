using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ElectroMVC.Models
{
    [PrimaryKey(nameof(OrderId), nameof(ProductId))]
    public class OrderDetail 
	{

        [Key]
        [Column(Order = 0)]
        public int OrderId { get; set; }
        [Key, Column(Order = 1)]
        public int  ProductId{ get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public virtual Order? Order { get; set; }
        public virtual Product? Product { get; set; }
    }
}

