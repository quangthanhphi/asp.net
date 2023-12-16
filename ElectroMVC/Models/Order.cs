using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectroMVC.Models
{
    public class Order : CommonAbstract
    {
        public Order() => this.orderDetails = new HashSet<OrderDetail>();

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Code { get; set; }

        public string? CustomerName { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? Email { get; set; }

        public int Quantity { get; set; }
        
        public decimal TotalAmount { get; set; }

        public int TypePayment { get; set; }

        public int Status { get; set; }

        public ICollection<OrderDetail> orderDetails { get; set; }
    }
}
