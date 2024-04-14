namespace Task_Backend.DTO
{
    public class DocumentItemDto
    {
        public int DocumentId { get; set; }
        public int Ordinal { get; set; }
        public string? Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int TaxRate { get; set; }
    }

}
