using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Backend.Models
{
    public class Document
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters.")]
        public string Type { get; set; } = null!;

        [Required]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "First name cannot exceed 30 characters.")]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(30, ErrorMessage = "Last name cannot exceed 30 characters.")]
        public string LastName { get; set; } = null!;

        [Required]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
        public string City { get; set; } = null!;

        // Relation to DocumentItem
        public virtual ICollection<DocumentItem> DocumentItems { get; set; } = new List<DocumentItem>();
    }
}