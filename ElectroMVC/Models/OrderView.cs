// OrderViewModel.cs
using System.Collections.Generic;

namespace ElectroMVC.Models
{
    public class OrderViewModel
    {
        public Order? Order { get; set; }
        public OrderDetail? OrderDetail { get; set; }
        public string? ProductName { get; set; }
        public Product? Product { get; set; }
        public int TypePaymentVN { get; set; }
      
    }
}
