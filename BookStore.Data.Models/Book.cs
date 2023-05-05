using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Data.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Copies { get; set; }
        public double Price { get; set; }
        public EditionType EditionType { get; set; }
        public AgeRestriction AgeRestriction { get; set; }
        public int AuthorId { get; set; }
        public virtual Author Author { get; set; } = null!;
        public virtual ICollection<BookCategory> BookCategories { get; } = new List<BookCategory>();
    }
}
