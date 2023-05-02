using System.ComponentModel.DataAnnotations;

namespace GroceryStore.Models
{
    public class Product
    {
        [Key]
        public int PrID { get; set; }
        public string PrName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
