// OrderViewModel.cs
using System.Collections.Generic;
using ElectroMVC.Models;

namespace ElectroMVC.Models
{
    public class OrderViewModel
    {
        public Order? Order { get; set; }
        public OrderDetail? OrderDetail { get; set; }
        public string? ProductName { get; set; }

        //public IEnumerator<OrderViewModel> GetEnumerator()
        //{
        //    yield return this;
        //}

        //System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        //{
        //    return GetEnumerator();
        //}
    }
}