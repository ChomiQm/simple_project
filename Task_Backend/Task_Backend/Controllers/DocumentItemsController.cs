using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task_Backend.Models;

namespace Task_Backend.Controllers
{
    [Route("documentitems")]
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

        [HttpGet("bydocument/{documentId}")]
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

        [HttpGet("details/{documentId}/{ordinal}")]
        public async Task<ActionResult<DocumentItem>> GetDocumentItem([FromRoute] int documentId, [FromRoute] int ordinal)
        {
            _logger.LogInformation($"Pobieranie DocumentItem dla DocumentId={documentId} i Ordinal={ordinal}.");
            var documentItem = await _context.DocumentItems.FindAsync(documentId, ordinal);

            if (documentItem == null)
            {
                _logger.LogWarning($"Nie znaleziono DocumentItem: DocumentId={documentId}, Ordinal={ordinal}.");
                return NotFound();
            }

            return documentItem;
        }

        [HttpPost("additem")]
        public async Task<ActionResult<DocumentItem>> PostDocumentItem([FromBody] DocumentItem documentItem)
        {
            _logger.LogInformation($"Dodawanie nowego DocumentItem dla DocumentId={documentItem.DocumentId}.");
            _context.DocumentItems.Add(documentItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDocumentItem", new { documentId = documentItem.DocumentId, ordinal = documentItem.Ordinal }, documentItem);
        }

        [HttpPut("update{documentId}/item/{ordinal}")]
        public async Task<IActionResult> PutDocumentItem([FromRoute] int documentId, [FromRoute] int ordinal, [FromBody] DocumentItem documentItem)
        {
            if (documentId != documentItem.DocumentId || ordinal != documentItem.Ordinal)
            {
                _logger.LogWarning($"Nieprawidłowe dane w żądaniu: DocumentId={documentId}, Ordinal={ordinal}.");
                return BadRequest("Invalid DocumentId or Ordinal.");
            }

            _context.Entry(documentItem).State = EntityState.Modified;
            _logger.LogInformation($"Aktualizacja DocumentItem dla DocumentId={documentId} i Ordinal={ordinal}.");

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!DocumentItemExists(documentId, ordinal))
                {
                    _logger.LogWarning($"Nie znaleziono DocumentItem podczas aktualizacji: DocumentId={documentId}, Ordinal={ordinal}.");
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, $"Błąd konkurencji podczas aktualizacji DocumentItem: DocumentId={documentId}, Ordinal={ordinal}.");
                    throw;
                }
            }

            return NoContent();
        }


        [HttpDelete("delete{documentId}/item/{ordinal}")]
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
