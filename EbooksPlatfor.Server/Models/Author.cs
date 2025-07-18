using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Biography { get; set; }
        public ICollection<Book>? Books { get; set; }
    }
}
