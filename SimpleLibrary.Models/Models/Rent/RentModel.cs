using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLibrary.Models.Models.Rent
{
    public class RentModel
    {
        public int RentId { get; set; }
        public string UserName { get; set; }
        public string BookName { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public bool Status { get; set; }
        public string RentAt { get; set; }
        public string ReturnedAt { get; set; }
    }
}
