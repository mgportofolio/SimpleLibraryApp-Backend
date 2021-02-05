using System;
using System.Collections.Generic;

#nullable disable

namespace SimpleLibraryDbContext.Entities
{
    public partial class Rent
    {
        public int RentId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime RentAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Book Book { get; set; }
        public virtual User User { get; set; }
    }
}
