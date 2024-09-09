using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagementSystem.Models
{
    public class Purchase
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }
    }
}
