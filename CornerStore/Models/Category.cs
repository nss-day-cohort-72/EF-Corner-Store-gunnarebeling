using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CornerStore.Models;

public class Category
{  
    public int Id { get; set; }
    public string CategoryName { get; set; }
    public List<Product> Products { get; set; }
    
}