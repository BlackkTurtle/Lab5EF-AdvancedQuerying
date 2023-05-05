using System.Text;
using BookShop.Data;
using BookStore.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ILogger<CategoriesController> _logger;
        private BookShopContext context;
        public CategoriesController(ILogger<CategoriesController> logger,
            BookShopContext context)
        {
            _logger = logger;
            this.context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categories>>> GetAllCategoriesAsync()
        {
            try
            {
                var results = await context.categories.ToListAsync();
                _logger.LogInformation($"Отримали всі дані з бази даних!");
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Транзакція сфейлилась! Щось пішло не так у методі - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "вот так вот!");
            }
        }
        [HttpGet("TotalProfit")]
        public async Task<ActionResult> GetTotalProfitByCategoryAsync()
        {
            try
            {
                var categories = await context.categories.ToListAsync();
                var books= await context.books.ToListAsync();
                var bookcategories=await context.bookCategories.ToListAsync();
                var entity = from c in categories
                             join bc in bookcategories on c.CategoryId equals bc.CategoryId
                             join b in books on bc.BookId equals b.BookId
                             select new
                             {
                                 b.Copies,
                                 b.Price,
                                 c.CategoryName
                             };
                var results = entity.GroupBy(e => e.CategoryName).Select(g => new
                {
                    g.Key,
                    SUM=g.Sum(s=>s.Copies*s.Price)
                });
                _logger.LogInformation($"Отримали всі дані з бази даних!");
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Транзакція сфейлилась! Щось пішло не так у методі - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "вот так вот!");
            }
        }
        [HttpGet("RecentBooks")]
        public async Task<ActionResult> GetRecentCategoriesAsync()
        {
            try
            {
                var categories = await context.categories.ToListAsync();
                var books = await context.books.ToListAsync();
                var bookcategories = await context.bookCategories.ToListAsync();
                var entity = from c in categories
                             join bc in bookcategories on c.CategoryId equals bc.CategoryId
                             join b in books on bc.BookId equals b.BookId
                             select new
                             {
                                 CategoryName=c.CategoryName,
                                 Books = books.Select(bb => new
                                 {
                                     Title=bb.Title,
                                     ReleaseDate=bb.ReleaseDate
                                 }).OrderByDescending(e=>e.ReleaseDate).Take(3)
                             };

                StringBuilder sb = new StringBuilder();
                foreach (var category in entity)
                {
                    sb.AppendLine($"--{category.CategoryName}");
                    foreach (var book in category.Books)
                    {
                        sb.AppendLine($"{book.Title} ({book.ReleaseDate.Year})");
                    }
                }
                _logger.LogInformation($"Отримали всі дані з бази даних!");
                return Ok(sb.ToString().Trim());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Транзакція сфейлилась! Щось пішло не так у методі - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "вот так вот!");
            }
        }
        [HttpGet("id/{id}")]
        public async Task<ActionResult<Categories>> GetCategoriesByIdAsync(int id)
        {
            try
            {
                var entity = await context.categories.Where(e => e.CategoryId == id).SingleOrDefaultAsync();
                if (entity == null)
                {
                    _logger.LogInformation($"Id: {id}, не був знайдейний у базі даних");
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
        public async Task<ActionResult> PostCategoriesAsync([FromBody] Categories fullentity)
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
                var entity = new Categories()
                {
                    CategoryName=fullentity.CategoryName
                };
                await context.categories.AddAsync(entity);
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
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCategoriesAsync(int id, [FromBody] Categories updatedentity)
        {
            try
            {
                if (updatedentity == null)
                {
                    _logger.LogInformation($"Empty JSON received from the client");
                    return BadRequest("object is null");
                }

                var entity = await context.categories.Where(e => e.CategoryId == id).SingleOrDefaultAsync();
                if (entity == null)
                {
                    _logger.LogInformation($"ID: {id} was not found in the database");
                    return NotFound();
                }
                entity.CategoryName = updatedentity.CategoryName;
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
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategoriesByIdAsync(int id)
        {
            try
            {
                var entity = await context.categories.Where(e => e.CategoryId == id).SingleOrDefaultAsync();
                if (entity == null)
                {
                    _logger.LogInformation($"Id: {id}, не був знайдейний у базі даних");
                    return NotFound();
                }

                context.categories.Remove(entity);
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
