using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLibrary.Models.Models.Book
{
    public class BookDetailModel
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string AuthorName { get; set; }
        public string BookGenre { get; set; }
        public string BookDescription { get; set; }
    }
}
