using System.Linq;
using System.Xml.Linq;
using BookShop.Data;
using BookStore.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly ILogger<BookController> _logger;
        private BookShopContext context;
        public BookController(ILogger<BookController> logger,
            BookShopContext context)
        {
            _logger = logger;
            this.context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetAllBookAsync()
        {
            try
            {
                var results = await context.books.ToListAsync();
                _logger.LogInformation($"Отримали всі дані з бази даних!");
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Транзакція сфейлилась! Щось пішло не так у методі - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "вот так вот!");
            }
        }
        [HttpGet("AgeRestriction/{ar}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByAgeRestriction(AgeRestriction ar)
        {
            try
            {
                var entity = await context.books.Where(e => e.AgeRestriction == ar).ToListAsync();
                if (entity == null)
                {
                    _logger.LogInformation($"Id: {ar}, не був знайдейний у базі даних");
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
        [HttpGet("Price/{price}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByPrice(double price)
        {
            try
            {
                var entity = await context.books.Where(e => e.Price <= price).OrderByDescending(e => e.Price).ToListAsync();
                if (entity == null)
                {
                    _logger.LogInformation($"Id не був знайдейний у базі даних");
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
        [HttpGet("Year/{year}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksNotReleasedIn(int year)
        {
            try
            {
                var entity = await context.books.Where(e => e.ReleaseDate.Year != year).OrderBy(e => e.BookId).ToListAsync();
                if (entity == null)
                {
                    _logger.LogInformation($"Id не був знайдейний у базі даних");
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
        [HttpGet("copies/{copies}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetGoldenBooks(int copies)
        {
            try
            {
                var entity = await context.books.Where(e => e.Copies <= copies).OrderByDescending(e=>e.BookId).ToListAsync();
                if (entity == null)
                {
                    _logger.LogInformation($"Id: {copies}, не був знайдейний у базі даних");
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
        [HttpGet("categories/{categories}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByCategory(string categories)
        {
            try
            {
                var categoriesarr=categories.Split(' ');
                var books = await context.books.ToListAsync();
                var categoriesable = await context.categories.Where(e=>categoriesarr.Contains(e.CategoryName)).ToListAsync();
                var BookCategory=await context.bookCategories.ToListAsync();

                var entity = from c in categoriesable
                            join bc in BookCategory on c.CategoryId equals bc.CategoryId
                            join b in books on bc.BookId equals b.BookId
                            select new
                            {
                                b.BookId,
                                b.Title,
                                b.Description,
                                b.ReleaseDate,
                                b.Copies,
                                b.Price,
                                b.EditionType,
                                b.AgeRestriction,
                                b.AuthorId
                            };
                entity.OrderBy(e => e.Title);
                if (entity == null)
                {
                    _logger.LogInformation($"Id не був знайдейний у базі даних");
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
        [HttpGet("date/{date}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksReleasedBefore(string date)
        {
            try
            {
                var date1=DateTime.ParseExact(date,"dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                var entity = await context.books.Where(e => e.ReleaseDate < date1).OrderBy(e => e.BookId).ToListAsync();
                if (entity == null)
                {
                    _logger.LogInformation($"Id не був знайдейний у базі даних");
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
        [HttpGet("name/{name}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksTitlesContainig(string name)
        {
            try
            {
                var entity = await context.books.Where(e => e.Title.Contains(name)).ToListAsync();
                if (entity == null)
                {
                    _logger.LogInformation($"Id не був знайдейний у базі даних");
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
        [HttpGet("authorname/{authorname}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByAuthor(string authorname)
        {
            try
            {
                var authors = await context.authors.Where(e => e.FirstName.Contains(authorname)).ToListAsync();
                var books = await context.books.ToListAsync();
                var entity = from b in books
                             join a in authors on b.AuthorId equals a.AuthorId
                             select new
                             {
                                 b.BookId,
                                 b.Title,
                                 b.Description,
                                 b.ReleaseDate,
                                 b.Copies,
                                 b.Price,
                                 b.EditionType,
                                 b.AgeRestriction,
                                 b.AuthorId
                             };
                if (entity == null)
                {
                    _logger.LogInformation($"Id не був знайдейний у базі даних");
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
        [HttpGet("count/{count}")]
        public async Task<ActionResult<string>> BooksCount(int count)
        {
            try
            {
                var entity = await context.books.Where(e => e.Title.Length>count).ToListAsync();
                if (entity == null)
                {
                    _logger.LogInformation($"Id не був знайдейний у базі даних");
                    return NotFound();
                }
                else
                {
                    _logger.LogInformation($"Отримали з бази даних!");
                    string result = $"There are {entity.Count} books with longer title than {count} characters";
                    return Ok(result);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Транзакція сфейлилась! Щось пішло не так у методі - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "вот так вот!");
            }
        }
        [HttpGet("count")]
        public async Task<ActionResult> CountCopiesByAuthor()
        {
            try
            {
                var books = await context.books.ToListAsync();
                var results = from b in books
                              group b by b.AuthorId into g
                              select new
                              {
                                  AuthorId = g.Key,
                                  count = g.Count()
                              };
                _logger.LogInformation($"Отримали всі дані з бази даних!");
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Транзакція сфейлилась! Щось пішло не так у методі - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "вот так вот!");
            }
        }
        [HttpGet("id/{id}")]
        public async Task<ActionResult<Book>> GetBookByIdAsync(int id)
        {
            try
            {
                var entity = await context.books.Where(e => e.BookId == id).SingleOrDefaultAsync();
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
        public async Task<ActionResult> PostBookAsync([FromBody] Book fullentity)
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
                var entity = new Book()
                {
                    Title = fullentity.Title,
                    Description = fullentity.Description,
                    ReleaseDate = fullentity.ReleaseDate,
                    Copies = fullentity.Copies,
                    Price = fullentity.Price,
                    EditionType = fullentity.EditionType,
                    AgeRestriction = fullentity.AgeRestriction,
                    AuthorId=fullentity.AuthorId
                };
                await context.books.AddAsync(entity);
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
        public async Task<ActionResult> UpdateBookAsync(int id, [FromBody] Book updatedentity)
        {
            try
            {
                if (updatedentity == null)
                {
                    _logger.LogInformation($"Empty JSON received from the client");
                    return BadRequest("object is null");
                }

                var entity = await context.books.Where(e => e.BookId == id).SingleOrDefaultAsync();
                if (entity == null)
                {
                    _logger.LogInformation($"ID: {id} was not found in the database");
                    return NotFound();
                }
                entity.Title = updatedentity.Title;
                entity.Description = updatedentity.Description;
                entity.AuthorId = updatedentity.AuthorId;
                entity.AgeRestriction = updatedentity.AgeRestriction;
                entity.ReleaseDate = updatedentity.ReleaseDate;
                entity.Copies=updatedentity.Copies;
                entity.Price=updatedentity.Price;
                entity.EditionType = updatedentity.EditionType;
                await context.SaveChangesAsync();
                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Transaction failed! Something went wrong in method - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error occurred.");
            }
        }
        [HttpPut("IncreaseYear/{year}")]
        public async Task<ActionResult> IncreasePricesAsync(int year)
        {
            try
            {
                var entity = await context.books.Where(e => e.ReleaseDate.Year <year).ToListAsync();
                if (entity == null)
                {
                    _logger.LogInformation($"ID was not found in the database");
                    return NotFound();
                }
                foreach (var book in entity)
                {
                    book.Price -= 5;
                }
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
        public async Task<ActionResult> DeleteBookByIdAsync(int id)
        {
            try
            {
                var entity = await context.books.Where(e => e.BookId == id).SingleOrDefaultAsync();
                if (entity == null)
                {
                    _logger.LogInformation($"Id: {id}, не був знайдейний у базі даних");
                    return NotFound();
                }

                context.books.Remove(entity);
                await context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Транзакція сфейлилась! Щось пішло не так у методі - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "вот так вот!");
            }
        }
        [HttpDelete("RemoveBooks/{copies}")]
        public async Task<ActionResult> RemoveBooks(int copies)
        {
            try
            {
                var entity = await context.books.Where(e => e.Copies<copies).ToListAsync();
                if (entity == null)
                {
                    _logger.LogInformation($"Id не був знайдейний у базі даних");
                    return NotFound();
                }
                int i=0;
                foreach (var book in entity)
                {
                    context.books.Remove(book);
                    i++;
                }
                await context.SaveChangesAsync();
                return Ok(i+" books were deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Транзакція сфейлилась! Щось пішло не так у методі - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "вот так вот!");
            }
        }
    }
}
