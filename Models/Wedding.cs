using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId { get; set; }


        [Display(Prompt = "Wedder One")]
        public string WedderOne { get; set; }


        [Display(Prompt = "Wedder Two")]
        public string WedderTwo { get; set; }



        public DateTime WeddingDay { get; set; }


        [Display(Prompt = "Address")]
        public string Address { get; set; }



        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;


        // Relashinship-----------------------------------------
        // Foregin Key for O2M
        public int UserId { get; set; }
        // Nav Property for O2M
        public User User { get; set; }


        //Nav Property for guest list
        public List<GuestList> GuestLists { get; set; }




    }
}