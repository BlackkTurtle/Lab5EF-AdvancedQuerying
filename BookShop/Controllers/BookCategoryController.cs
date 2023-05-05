using BookShop.Data;
using BookStore.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookCategoryController : ControllerBase
    {
        private readonly ILogger<BookCategoryController> _logger;
        private BookShopContext context;
        public BookCategoryController(ILogger<BookCategoryController> logger,
            BookShopContext context)
        {
            _logger = logger;
            this.context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookCategory>>> GetAllBookCategoryAsync()
        {
            try
            {
                var results = await context.bookCategories.ToListAsync();
                _logger.LogInformation($"Отримали всі дані з бази даних!");
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Транзакція сфейлилась! Щось пішло не так у методі - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "вот так вот!");
            }
        }
        [HttpGet("id/{id1}/{id2}")]
        public async Task<ActionResult<BookCategory>> GetBookCategoryByIdAsync(int id1,int id2)
        {
            try
            {
                var entity = await context.bookCategories.Where(e => e.BookId == id1 && e.CategoryId==id2).SingleOrDefaultAsync();
                if (entity == null)
                {
                    _logger.LogInformation($"Id: {id1} {id2}, не був знайдейний у базі даних");
                    return NotFound();
                }
                else
                {
                    _logger.LogInformation($"Отримали з бази даних!");
                    return Ok(entity);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Транзакція сфейлилась! Щось пішло не так у методі - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "вот так вот!");
            }
        }

        //POST: api/events
        [HttpPost]
        public async Task<ActionResult> PostBookCategoryAsync([FromBody] BookCategory fullentity)
        {
            try
            {
                if (fullentity == null)
                {
                    _logger.LogInformation($"Ми отримали пустий json зі сторони клієнта");
                    return BadRequest("Обєкт є null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogInformation($"Ми отримали некоректний json зі сторони клієнта");
                    return BadRequest("Обєкт є некоректним");
                }
                var entity = new BookCategory()
                {
                    BookId=fullentity.BookId,
                    CategoryId=fullentity.CategoryId,
                };
                await context.bookCategories.AddAsync(entity);
                await context.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Транзакція сфейлилась! Щось пішло не так у методі - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "вот так вот!");
            }
        }

        //POST: api/events/id
        [HttpPut("{id1}/{id2}")]
        public async Task<ActionResult> UpdateBookCategoryAsync(int id1,int id2, [FromBody] BookCategory updatedentity)
        {
            try
            {
                if (updatedentity == null)
                {
                    _logger.LogInformation($"Empty JSON received from the client");
                    return BadRequest("object is null");
                }

                var entity = await context.bookCategories.Where(e => e.BookId == id1 && e.CategoryId == id2).SingleOrDefaultAsync();
                if (entity == null)
                {
                    _logger.LogInformation($"ID: {id1} was not found in the database");
                    return NotFound();
                }
                entity.BookId = updatedentity.BookId;
                entity.CategoryId = updatedentity.CategoryId;
                await context.SaveChangesAsync();
                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Transaction failed! Something went wrong in method - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error occurred.");
            }
        }

        //GET: api/events/Id
        [HttpDelete("{id1}/{id2}")]
        public async Task<ActionResult> DeleteBookCategoryByIdAsync(int id1,int id2)
        {
            try
            {
                var entity = await context.bookCategories.Where(e => e.BookId == id1 && e.CategoryId == id2).SingleOrDefaultAsync();
                if (entity == null)
                {
                    _logger.LogInformation($"Id: {id1}, не був знайдейний у базі даних");
                    return NotFound();
                }

                context.bookCategories.Remove(entity);
                await context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Транзакція сфейлилась! Щось пішло не так у методі - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "вот так вот!");
            }
        }
    }
}
