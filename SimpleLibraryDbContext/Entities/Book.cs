using System;
using System.Collections.Generic;

#nullable disable

namespace SimpleLibraryDbContext.Entities
{
    public partial class Book
    {
        public Book()
        {
            Rents = new HashSet<Rent>();
        }

        public int BookId { get; set; }
        public string BookName { get; set; }
        public string BookDescription { get; set; }
        public string BookAuthor { get; set; }
        public string BookGenre { get; set; }
        public string BookImage { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Rent> Rents { get; set; }
    }
}
