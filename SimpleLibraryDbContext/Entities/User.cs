using System;
using System.Collections.Generic;

#nullable disable

namespace SimpleLibraryDbContext.Entities
{
    public partial class User
    {
        public User()
        {
            Rents = new HashSet<Rent>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string UserRole { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Rent> Rents { get; set; }
    }
}
