using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_Backend.DTO;
using Task_Backend.Models;

namespace Task_Backend.Controllers
{
    [Route("documentItems")]
    [ApiController]
    public class DocumentItemsController : ControllerBase
    {
        private readonly ModelContext _context;
        private readonly ILogger<DocumentItemsController> _logger;

        public DocumentItemsController(ModelContext context, ILogger<DocumentItemsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("byDocument/{documentId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<DocumentItem>>> GetDocumentItemsByDocumentId([FromRoute] int documentId)
        {
            _logger.LogInformation($"Pobieranie pozycji dla dokumentu o ID: {documentId}.");

            var documentItems = await _context.DocumentItems
                                              .Where(di => di.DocumentId == documentId)
                                              .ToListAsync();

            if (!documentItems.Any())
            {
                _logger.LogWarning($"Nie znaleziono pozycji dla dokumentu o ID: {documentId}.");
                return NotFound();
            }

            _logger.LogInformation($"Zwrócono pozycje dla dokumentu o ID: {documentId}.");
            return documentItems;
        }

        [HttpGet("allItems")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<DocumentItem>>> GetAllDocumentItems()
        {
            _logger.LogInformation("Pobieranie wszystkich unikatowych pozycji dokumentów.");

            var documentItems = await _context.DocumentItems
                .GroupBy(item => item.Product)
                .Select(group => new DocumentItem
                {
                    Product = group.Key,
                    Price = group.Min(item => item.Price),
                    Quantity = group.Min(item => item.Quantity),
                    TaxRate = group.Min(item => item.TaxRate)
                })
                .ToListAsync();

            if (!documentItems.Any())
            {
                _logger.LogWarning("Nie znaleziono żadnych pozycji dokumentów.");
                return NotFound("No document items found.");
            }

            _logger.LogInformation($"Zwrócono {documentItems.Count} unikatowych pozycji dokumentów.");
            return documentItems;
        }

        [HttpPost("addItemToDocument/{documentId}")]
        [Authorize]
        public async Task<ActionResult<DocumentItemDto>> PostDocumentItemToDocument([FromRoute] int documentId, [FromBody] DocumentItemDto newItemDto)
        {
            if (newItemDto == null)
            {
                _logger.LogWarning("Przesłany DocumentItemDto jest null.");
                return BadRequest("DocumentItemDto cannot be null.");
            }

            if (documentId != newItemDto.DocumentId)
            {
                _logger.LogWarning("Nieprawidłowe ID dokumentu.");
                return BadRequest("The DocumentId in the route does not match the DocumentItemDto's DocumentId.");
            }

            var document = await _context.Documents.FindAsync(documentId);
            if (document == null)
            {
                _logger.LogWarning($"Dokument o ID {documentId} nie istnieje.");
                return NotFound($"Document with ID {documentId} does not exist.");
            }

            var ordinals = await _context.DocumentItems
                .Where(di => di.Product == newItemDto.Product)
                .Select(di => di.Ordinal)
                .ToListAsync();

            var newOrdinal = ordinals.DefaultIfEmpty(0).Max() + 1;

            var newItem = new DocumentItem
            {
                DocumentId = documentId,
                Ordinal = newOrdinal,
                Product = newItemDto.Product,
                Quantity = newItemDto.Quantity,
                Price = newItemDto.Price,
                TaxRate = newItemDto.TaxRate
            };

            _context.DocumentItems.Add(newItem);
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Dodano pozycję dokumentu: {newItem.Product} do dokumentu o ID: {documentId}.");

                var newItemDtoResponse = new DocumentItemDto
                {
                    DocumentId = newItem.DocumentId,
                    Ordinal = newItem.Ordinal,
                    Product = newItem.Product,
                    Quantity = newItem.Quantity,
                    Price = newItem.Price,
                    TaxRate = newItem.TaxRate
                };

                return CreatedAtAction(nameof(GetDocumentItemsByDocumentId), new { documentId = newItem.DocumentId }, newItemDtoResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wystąpił błąd podczas dodawania pozycji dokumentu.");
                return StatusCode(500, "An error occurred while adding the document item.");
            }
        }

        [HttpPut("update{documentId}/item/{ordinal}")]
        [Authorize]
        public async Task<IActionResult> PutDocumentItem([FromRoute] int documentId, [FromRoute] int ordinal, [FromBody] DocumentItemDto documentItemDto)
        {
            if (documentId != documentItemDto.DocumentId || ordinal != documentItemDto.Ordinal)
            {
                _logger.LogWarning($"Nieprawidłowe dane w żądaniu: DocumentId={documentId}, Ordinal={ordinal}.");
                return BadRequest("Invalid DocumentId or Ordinal.");
            }

            var documentItem = await _context.DocumentItems
                .FirstOrDefaultAsync(di => di.DocumentId == documentId && di.Ordinal == ordinal);

            if (documentItem == null)
            {
                _logger.LogWarning($"Nie znaleziono DocumentItem do aktualizacji: DocumentId={documentId}, Ordinal={ordinal}.");
                return NotFound();
            }

            documentItem.Product = documentItemDto.Product;
            documentItem.Quantity = documentItemDto.Quantity;
            documentItem.Price = documentItemDto.Price;
            documentItem.TaxRate = documentItemDto.TaxRate;

            _context.DocumentItems.Update(documentItem);
            _logger.LogInformation($"Aktualizacja DocumentItem dla DocumentId={documentId} i Ordinal={ordinal}.");

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Błąd konkurencji podczas aktualizacji DocumentItem: DocumentId={documentId}, Ordinal={ordinal}.");
                return StatusCode(500, "An error occurred while updating the document item.");
            }

            return NoContent();
        }

        [HttpDelete("delete{documentId}/item/{ordinal}")]
        [Authorize]
        public async Task<IActionResult> DeleteDocumentItem([FromRoute] int documentId, [FromRoute] int ordinal)
        {
            _logger.LogInformation($"Usuwanie DocumentItem dla DocumentId={documentId} i Ordinal={ordinal}.");
            var documentItem = await _context.DocumentItems.FindAsync(documentId, ordinal);
            if (documentItem == null)
            {
                _logger.LogWarning($"Nie znaleziono DocumentItem do usunięcia: DocumentId={documentId}, Ordinal={ordinal}.");
                return NotFound();
            }

            _context.DocumentItems.Remove(documentItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Usunięto DocumentItem: DocumentId={documentId}, Ordinal={ordinal}.");

            return NoContent();
        }

        private bool DocumentItemExists(int documentId, int ordinal)
        {
            return _context.DocumentItems.Any(e => e.DocumentId == documentId && e.Ordinal == ordinal);
        }
    }
}
