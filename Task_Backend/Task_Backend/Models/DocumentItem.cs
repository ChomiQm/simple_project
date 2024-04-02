using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Backend.Models
{
    public class DocumentItem
    {
        [Key, Column(Order = 0), Required]
        public int DocumentId { get; set; } // Foreign key to Document

        [Key, Column(Order = 1), Required]
        public int Ordinal { get; set; } // Second part of combined primary key

        [Required]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string Product { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Tax rate must be between 0 and 100.")]
        public int TaxRate { get; set; }

        // DocumentItem <Many> -> Document <1>
        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; } = null!;
    }
}