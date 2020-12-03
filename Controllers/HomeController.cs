using System;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;


namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {

        private MyContext _context;

        public HomeController(MyContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult index()
        {
            indexWrapper WMod = new indexWrapper();

            return View("index", WMod);
        }






        // Navigate to other pages--------------------------------- 

        [HttpGet("goToPlanWedding")]
        public IActionResult goToPlanWedding()
        {
            return RedirectToAction("planWedding");
        }

        [HttpGet("goToDashboard")]
        public IActionResult goToDashboard()
        {
            return RedirectToAction("dashboard");
        }


        // -----------------------------------------------------------end

        // Redering pages-----------------------------------------------
        [HttpGet("dashboard")]
        // dashboard
        public IActionResult dashboard()
        {
            // block pages is not in session
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("index");
            }

            // user in session  
            int UserIdInSession = (int)HttpContext.Session.GetInt32("UserId");
            // filter user in session with db
            User UserIndb = _context.Users.FirstOrDefault(u => u.UserId == UserIdInSession);
            ViewBag.DisplayUser = UserIndb;


            // Get all guest from db set
            List<Wedding> allPostedWeddings = _context.Weddings
            .Include(gl => gl.GuestLists)
            .ToList();



            return View("dashboard", allPostedWeddings);
        }






        // wedding page
        [HttpGet("planWedding")]

        public IActionResult planWedding()
        {

            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("index");
            }

            int UserIdInSession = (int)HttpContext.Session.GetInt32("UserId");

            User UserIndb = _context.Users.FirstOrDefault(u => u.UserId == UserIdInSession);
            ViewBag.DisplayUser = UserIndb;

            return View("planWedding");
        }


        [HttpGet("weddingInfo/{WeddingId}")]
        public IActionResult weddingInfo(int WeddingId)
        {

            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("index");
            }

            ViewBag.selectedWedding = _context.Weddings.FirstOrDefault(sw => sw.WeddingId == WeddingId);

            ViewBag.allGuests = _context.Users
            .Include(g => g.GuestLists)
            .Where(u => u.GuestLists.Any(gl => gl.WeddingId == WeddingId))
            .ToList();


            return View("weddingInfo");
        }





        // -----------------------------------------------------------end


        // Processing-----------------------------------------
        [HttpPost("CreateWedding")]
        public IActionResult CreateWedding(Wedding FromForm)
        {

            int GetUserIdInsession = (int)HttpContext.Session.GetInt32("UserId");

            FromForm.UserId = GetUserIdInsession;

            _context.Add(FromForm);
            _context.SaveChanges();


            System.Console.WriteLine("New wedding was created");
            return RedirectToAction("dashboard");
        }


        [HttpGet("delete/{WeddingId}")]
        public IActionResult delete(int WeddingId)
        {

            // Get item from db
            Wedding GetWedding = _context.Weddings.SingleOrDefault(w => w.WeddingId == WeddingId);

            System.Console.WriteLine(GetWedding);


            _context.Weddings.Remove(GetWedding);
            _context.SaveChanges();

            System.Console.WriteLine("Delete button was click");
            return RedirectToAction("dashboard");
        }

        [HttpGet("removeRSVP/{WeddingId}/{UserId}")]
        public IActionResult removeRSVP(int WeddingId, int UserId)
        {

            // int UserIdInSession = (int)HttpContext.Session.GetInt32("UserId");
            // if (_context.GuestLists.Any(g => g.GuestListId == UserIdInSession && g.WeddingId == WeddingId))
            // {
            //     System.Console.WriteLine("Block");
            //     return RedirectToAction("dashboard");
            // }


            GuestList RemoveGuest = _context.GuestLists
            .FirstOrDefault(r => r.UserId == UserId && r.WeddingId == WeddingId);

            _context.Remove(RemoveGuest);
            _context.SaveChanges();


            System.Console.WriteLine("UnRSVP button was click");
            return RedirectToAction("dashboard");
        }


        [HttpGet("rsvpUser/{WeddingId}/{UserId}")]
        public IActionResult rsvpUser(GuestList FromURL, int WeddingId, int UserId)
        {



            FromURL.WeddingId = WeddingId;
            FromURL.UserId = UserId;

            System.Console.WriteLine(WeddingId);
            System.Console.WriteLine(UserId);

            _context.Add(FromURL);
            _context.SaveChanges();

            System.Console.WriteLine("RSVP button Click");
            return RedirectToAction("dashboard");
        }







        // Registertion------------------------------------------------
        [HttpPost("Redgister")]
        public IActionResult Redgister(User FromForm)
        {

            // Pass in Models so that parcial can use them
            indexWrapper WMod = new indexWrapper();

            // Check if email is already in db
            if (_context.Users.Any(u => u.Email == FromForm.Email))
            {
                ModelState.AddModelError("Email", "Email already in use!");
            }

            // Validations
            if (ModelState.IsValid)
            {
                // #hash password
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                FromForm.Password = Hasher.HashPassword(FromForm, FromForm.Password);

                // Add to db
                _context.Add(FromForm);
                _context.SaveChanges();

                // Session
                HttpContext.Session.SetInt32("UserId", _context.Users.FirstOrDefault(i => i.UserId == FromForm.UserId).UserId);
                // Redirect
                System.Console.WriteLine("You may contine!");
                return RedirectToAction("dashboard", WMod);
            }
            else
            {
                System.Console.WriteLine("Fix your erros!");
                return View("index", WMod);

            }

        }



        // Login-------------------------------------------------    
        [HttpPost("Login")]
        public IActionResult Login(LoginUser userSubmission)
        {

            indexWrapper WMod = new indexWrapper();

            // Validations
            if (ModelState.IsValid)
            {

                // Check db email with from email
                var userInDb = _context.Users.FirstOrDefault(u => u.Email == userSubmission.LogEmail);

                // No user in db
                if (userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("index", WMod);
                }
                // Check hashing are the same
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LogPassword);

                if (result == 0)
                {
                    // ModelState.AddModelError("Email", "Invalid Email/Password");
                    // return View("index", WMod);// handle failure (this should be similar to how "existing email" is handled)
                }
                // Set Session Instance
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);

                return RedirectToAction("dashboard", WMod);

            }

            return View("index", WMod);

        }


        [HttpGet("logout")]
        public IActionResult logout()
        {
            // Clear Session
            HttpContext.Session.Clear();
            return RedirectToAction("index");
        }


        // ------------------------------------------end of regitration and login







    }

}