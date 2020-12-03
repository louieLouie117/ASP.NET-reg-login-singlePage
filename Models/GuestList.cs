using System;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class GuestList
    {
        [Key]
        public int GuestListId { get; set; }

        // id from user and Nav Property
        public int UserId { get; set; }
        public User User { get; set; }


        // Id From wedding and nav property
        public int WeddingId { get; set; }
        public Wedding Wedding { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;



    }
}