using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Task_Backend.DTO;
using Task_Backend.Models;

namespace Task_Backend.Controllers
{
    [Route("documents")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly ModelContext _context;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(ModelContext context, ILogger<DocumentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("getDocuments")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Document>>> GetDocuments(
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false)
        {
            _logger.LogInformation($"Pobieranie dokumentów strona {pageIndex} z rozmiarem {pageSize}.");
            var query = _context.Documents.AsQueryable();

            // Filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d =>
                    d.FirstName.Contains(search) ||
                    d.LastName.Contains(search) ||
                    d.Type.Contains(search) ||
                    d.City.Contains(search));
            }

            // Dynamic sorting using a dictionary
            var sortingExpressions = new Dictionary<string, Expression<Func<Document, object>>>
            {
                ["date"] = d => d.Date,
                ["lastname"] = d => d.LastName,
                ["firstname"] = d => d.FirstName,
                ["city"] = d => d.City,
                ["type"] = d => d.Type,
            };

            if (!string.IsNullOrEmpty(sortBy) && sortingExpressions.TryGetValue(sortBy.ToLower(), out var sortingExpression))
            {
                query = sortDescending ? query.OrderByDescending(sortingExpression) : query.OrderBy(sortingExpression);
            }
            else
            {
                query = sortDescending ? query.OrderByDescending(d => d.Id) : query.OrderBy(d => d.Id); // Default sorting
            }

            // Pagination
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var documents = await query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Response with metadata
            var response = new
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = documents
            };
            _logger.LogInformation($"Zwrócono {documents.Count} dokumentów, strona {pageIndex} z {totalPages}.");
            return Ok(response);
        }

        [HttpGet("getDocument/{id}")]
        [Authorize]
        public async Task<ActionResult<DocumentDto>> GetDocument([FromRoute] int id)
        {
            var document = await _context.Documents
                .Where(d => d.Id == id)
                .Select(d => new DocumentDto
                {
                    Id = d.Id,
                    Type = d.Type,
                    Date = d.Date,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    City = d.City,
                    DocumentItems = (ICollection<DocumentItem>)d.DocumentItems.Select(di => new DocumentItemDto
                    {
                        DocumentId = di.DocumentId,
                        Ordinal = di.Ordinal,
                        Product = di.Product,
                        Quantity = di.Quantity,
                        Price = di.Price,
                        TaxRate = di.TaxRate
                    }).ToList()
                })
                .SingleOrDefaultAsync();

            if (document == null)
            {
                return NotFound();
            }

            return document;
        }

        [HttpPut("updateDocument/{id}")]
        [Authorize]
        public async Task<IActionResult> PutDocument([FromRoute] int id, [FromBody] DocumentDto updateDto)
        {
            _logger.LogInformation("Rozpoczęcie aktualizacji dokumentu o identyfikatorze {DocumentId}.", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Dane wejściowe dla aktualizacji dokumentu są nieprawidłowe.");
                return BadRequest(ModelState);
            }

            var dbDocument = await _context.Documents.FindAsync(id);
            if (dbDocument == null)
            {
                _logger.LogWarning("Nie znaleziono dokumentu o identyfikatorze {DocumentId} do aktualizacji.", id);
                return NotFound();
            }

            // Mapowanie właściwości, które mogą być aktualizowane
            dbDocument.Type = updateDto.Type;
            dbDocument.Date = updateDto.Date;
            dbDocument.FirstName = updateDto.FirstName;
            dbDocument.LastName = updateDto.LastName;
            dbDocument.City = updateDto.City;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Dokument o identyfikatorze {DocumentId} został zaktualizowany.", id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!DocumentExists(id))
                {
                    _logger.LogWarning("Dokument o identyfikatorze {DocumentId} już nie istnieje.", id);
                    return NotFound();
                }
                _logger.LogError(ex, "Błąd współbieżności przy aktualizacji dokumentu o identyfikatorze {DocumentId}.", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wystąpił nieoczekiwany błąd podczas aktualizacji dokumentu o identyfikatorze {DocumentId}.", id);
                return StatusCode(500, "Internal server error - unable to update document");
            }

            return NoContent();
        }

        [HttpPost("addDocument")]
        [Authorize]
        public async Task<ActionResult<Document>> PostDocument([FromBody] Document document)
        {
            _logger.LogInformation("Rozpoczęcie dodawania nowego dokumentu.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Dane wejściowe dla nowego dokumentu są nieprawidłowe.");
                return BadRequest(ModelState);
            }

            try
            {
                _context.Documents.Add(document);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Dodano dokument: {document.Id}.");
                var response = new
                {
                    Message = "Document added successfully",
                    DocumentId = document.Id,
                };

                return CreatedAtAction(nameof(GetDocument), new { id = document.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd przy dodawaniu dokumentu.");
                return StatusCode(500, "Internal server error - unable to add document");
            }
        }

        [HttpDelete("deleteDocument/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteDocument([FromRoute] int id)
        {
            _logger.LogInformation($"Próba usunięcia dokumentu o ID: {id}.");
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                _logger.LogWarning($"Dokument o ID: {id} nie został znaleziony.");
                return NotFound();
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Dokument o ID: {id} został usunięty.");

            return NoContent();
        }

        [HttpDelete("deleteAll")]
        [Authorize]
        public async Task<IActionResult> DeleteAllDocuments([FromQuery] string confirmation)
        {
            _logger.LogWarning("Rozpoczęcie operacji usuwania wszystkich dokumentów.");

            if (confirmation != "CONFIRM")
            {
                _logger.LogWarning("Próba usunięcia wszystkich dokumentów bez potwierdzenia.");
                return BadRequest("Needs confirmation.");
            }

            try
            {
                var allDocuments = await _context.Documents.ToListAsync();
                _context.Documents.RemoveRange(allDocuments);

                await _context.Database.ExecuteSqlRawAsync($"DBCC CHECKIDENT ('dbo.{nameof(_context.Documents)}', RESEED, 0)");
                _logger.LogInformation("Auto-increment value for 'Documents' has been reset.");

                await _context.SaveChangesAsync();

                _logger.LogInformation("Wszystkie dokumenty zostały usunięte.");
                return Ok(new { message = "All documents have been deleted. Identity has been reset" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas usuwania wszystkich dokumentów.");
                return StatusCode(500, "Internal server error - unable to delete all documents");
            }
        }

        private bool DocumentExists(int id)
        {
            return _context.Documents.Any(e => e.Id == id);
        }
    }
}
