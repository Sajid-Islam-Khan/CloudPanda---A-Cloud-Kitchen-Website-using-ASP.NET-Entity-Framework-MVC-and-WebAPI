using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using CloudPandaNew.Context;
using System.Data.Entity;
using CloudPanda.Models.ViewModels;


namespace CloudPanda.Controllers
{
    public class UserController : Controller
    {
        CloudPandaEntities _dbContext;

        public UserController()
        {
            this._dbContext = new CloudPandaEntities();
        }

       
        public ActionResult Delete(int id)
        {
            var data = _dbContext.Users.Where(x => x.Id == id).First();
            _dbContext.Users.Remove(data);
            _dbContext.SaveChanges();

            return RedirectToAction("MyProfile");
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User u)
        {
            ModelState.Remove("RePassword");
            ModelState.Remove("Email");
            ModelState.Remove("FullName");
            ModelState.Remove("Area");
            ModelState.Remove("Address");
            ModelState.Remove("PhoneNumber");
            ModelState.Remove("Role");

            if (ModelState.IsValid)
            {
                var data = _dbContext.Users.Where(x => x.Username == u.Username && x.Password == u.Password).FirstOrDefault();
                if (data != null)
                {
                    Session["id"] = data.Id;
                    Session["username"] = data.Username;
                    Session["role"] = data.Role;
                    Session["area"] = data.Area;

                    if (data.Role == "buyer")
                    {
                        return RedirectToAction("BuyerDashboard", "User");
                    }
                    else if (data.Role == "seller")
                    {
                        return RedirectToAction("SellerDashboard", "User");
                    }
                    else if (data.Role == "admin")
                    {
                        return RedirectToAction("AdminDashboard", "User");
                    }

                    return RedirectToAction("Index", "User");
                }
                ViewBag.Invalid = "Invalid User";
                ModelState.Clear();
            }

            ModelState.Clear();
            //return View();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            TempData["Logout"] = "Logged out from the system";

            return View("Login");
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(User u)
        {


            if (_dbContext.Users.Any(x => x.Username == u.Username))
            {
                ModelState.Clear();
                return View();
            }

            _dbContext.Users.Add(u);

            _dbContext.SaveChanges();

            return View("Login");
        }

        public ViewResult ClearLogin()
        {
            ModelState.Clear();
            Session.Clear();
            return View("Login");
        }

        public ViewResult ClearSignUp()
        {
            ModelState.Clear();
            return View("SignUp");
        }

        public ActionResult BuyerDashboard()
        {
            if (Session["role"] != null)
            {
                var currentUserRole = Session["role"].ToString();
                if (currentUserRole != "buyer")
                {
                    return RedirectToAction("Login", "User");
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

            string buyerArea = Session["area"].ToString();
            var sellersByArea = _dbContext.Users.Where(x => x.Area == buyerArea && x.Role == "seller").Include(x => x.SellerFranchises).ToList();
            var latestFranchises = _dbContext.Franchises.OrderByDescending(x => x.DateAdded).ToList();
            var viewModel = new BuyerViewModel { Sellers = sellersByArea, Franchises = latestFranchises };
            return View(viewModel);
        }


        public ActionResult SellerDashboard()
        {
            if (Session["role"] != null)
            {
                var currentUserRole = Session["role"].ToString();
                if (currentUserRole != "seller")
                {
                    return RedirectToAction("Login", "User");
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }


            int sellerId = Convert.ToInt32(Session["id"]);

            var orders = _dbContext.Orders
                .Include(o => o.OrderMeal)
                .Include(o => o.OrderMeal.Meals)
                .Include(o => o.User)
                .Where(o => o.SellerId == sellerId)
                .ToList();
            var viewmodel = new SellerViewModel { Orders = orders };
            return View(viewmodel);
        }

        public ActionResult AdminDashboard()
        {
            if (Session["role"] != null) 
            { 
                var currentUserRole = Session["role"].ToString();
                if (currentUserRole != "admin")
                    {
                        return RedirectToAction("Login", "User");
                    }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

            var totalUsers = _dbContext.Users.Count();
            var totalMeals = _dbContext.Meals.Count();
            var totalFranchises = _dbContext.Franchises.Count();
            var totalOrders = _dbContext.Orders.Count();

            var viewModel = new AdminViewModel { Franchises = totalFranchises, Meals = totalMeals, Users = totalUsers, Orders = totalOrders };

            return View(viewModel);
        }

        public ActionResult MyProfile()
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userId = Convert.ToInt32(Session["id"]);
            var userList = _dbContext.Users.Where(x => x.Id == userId).ToList();

            return View(userList);
        }

        // Action for editing user profile
        [HttpGet]
        public ActionResult EditProfile()
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userId = (int)Session["id"];
            var user = _dbContext.Users.Find(userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult EditProfile(User updatedUser)
        {

            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (ModelState.IsValid)
            {
                var userId = (int?)Session["id"];

                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var user = _dbContext.Users.Find(userId);

                if (user == null)
                {
                    return HttpNotFound();
                }

                // Update only editable fields
                user.FullName = updatedUser.FullName;
                user.Username = updatedUser.Username;
                user.Password = updatedUser.Password;
                user.Email = updatedUser.Email;
                user.PhoneNumber = updatedUser.PhoneNumber;
                user.Address = updatedUser.Address;
                user.Area = updatedUser.Area;

                _dbContext.SaveChanges();
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("MyProfile");
            }

            TempData["ErrorMessage"] = "There was an error updating your profile.";
            return View("EditProfile", updatedUser);
        }

        public ActionResult BuyerProfile()
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userId = Convert.ToInt32(Session["id"]);
            var userList = _dbContext.Users.Where(x => x.Id == userId).ToList();

            return View(userList);
        }


        [HttpGet]
        public ActionResult EditBuyerProfile()
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userId = (int)Session["id"];
            var user = _dbContext.Users.Find(userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult EditBuyerProfile(User updatedUser)
        {

            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (ModelState.IsValid)
            {
                var userId = (int?)Session["id"];

                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var user = _dbContext.Users.Find(userId);

                if (user == null)
                {
                    return HttpNotFound();
                }

                // Update only editable fields
                user.FullName = updatedUser.FullName;
                user.Username = updatedUser.Username;
                user.Password = updatedUser.Password;
                user.Email = updatedUser.Email;
                user.PhoneNumber = updatedUser.PhoneNumber;
                user.Address = updatedUser.Address;
                user.Area = updatedUser.Area;

                _dbContext.SaveChanges();
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("BuyerProfile");
            }

            TempData["ErrorMessage"] = "There was an error updating your profile.";
            return View("EditBuyerProfile", updatedUser);
        }

    }
}