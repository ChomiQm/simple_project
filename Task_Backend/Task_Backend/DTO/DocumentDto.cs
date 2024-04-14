using Task_Backend.Models;

namespace Task_Backend.DTO
{
    public class DocumentDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = null!;
        public DateTime Date { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string City { get; set; } = null!;
        public virtual ICollection<DocumentItem>? DocumentItems { get; set; }
    }
}
