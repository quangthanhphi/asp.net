// ProductViewModel.cs
using System.Collections.Generic;
using ElectroMVC.Models;

public class ProductViewModel : IEnumerable<Product>
{
    public Product? Product { get; set; }
    public string? ProductCategoryName { get; set; }

    // Implement IEnumerable<T> by providing a GetEnumerator method
    public IEnumerator<Product> GetEnumerator()
    {
        // You can yield return the Product property or modify it based on your needs
        yield return Product;
    }

    // Explicit implementation of the non-generic IEnumerable
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
