using System;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class LoginUser
    {
        // No other fields!
        public int UserId { get; set; }
        public string LogEmail { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string LogPassword { get; set; }
    }
}



