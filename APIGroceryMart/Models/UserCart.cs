using System.ComponentModel.DataAnnotations;

namespace GroceryStore.Models
{
    public class UserCart
    {
        [Key]
        public int CartId { get; set; }
        // public string UserID { get; set; }
        public string UserID { get; set; }

        public string prName { get; set; }

        public int Quantity { get; set; }
        public decimal Amount { get; set; }
    }
}
