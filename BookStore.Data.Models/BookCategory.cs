using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Data.Models
{
    public class BookCategory
    {
        public int CategoryId { get; set; }
        public int BookId { get; set; }
        public virtual Book Book { get; set; } = null!;
        public virtual Categories Categories { get; set; } = null!;
    }
}
