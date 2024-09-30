using CloudPandaNew.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using CloudPanda.Models.ViewModels;

namespace CloudPandaNew.Controllers
{
    public class OrderController : Controller
    {

        CloudPandaEntities _dbContext;

        public OrderController()
        {
            this._dbContext = new CloudPandaEntities();
        }

        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuyerPurchaseMeals()
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
            var viewModel = new PurchaseMealViewModel { Sellers = sellersByArea };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddToCart(int sellerId, int mealId, int quantity)
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


            int buyerId = Convert.ToInt32(Session["id"]);


            var meal = _dbContext.Meals.FirstOrDefault(m => m.Id == mealId);
            if (meal == null)
            {
                return HttpNotFound("Meal not found");
            }


            var orderMeal = new OrderMeal
            {
                Quantity = quantity,
            };


            orderMeal.Meals.Add(meal);


            var order = new Order
            {
                BuyerId = buyerId,
                SellerId = sellerId,
                OrderTime = DateTime.Now,
                OrderMeal = orderMeal,
                Status = 0
            };


            _dbContext.OrderMeals.Add(orderMeal);
            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();

            return RedirectToAction("Cart", "Order");
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int orderId, int newQuantity)
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

            var order = _dbContext.Orders.Include(o => o.OrderMeal).FirstOrDefault(o => o.Id == orderId);

            if (order != null && newQuantity > 0)
            {

                order.OrderMeal.Quantity = newQuantity;
                _dbContext.SaveChanges();
            }


            return RedirectToAction("Cart");
        }

        [HttpPost]
        public ActionResult RemoveFromCart(int orderId)
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


            var order = _dbContext.Orders.Find(orderId);

            if (order != null)
            {

                _dbContext.Orders.Remove(order);
                _dbContext.SaveChanges();
            }


            return RedirectToAction("Cart");
        }




        public ActionResult Cart()
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


            int buyerId = Convert.ToInt32(Session["id"]);
            var cartItems = _dbContext.Orders
                .Where(o => o.BuyerId == buyerId && o.Status == 0) 
                .ToList();

            return View(cartItems);
        }

        [HttpPost]
        public ActionResult Checkout()
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


            var buyerId = Convert.ToInt32(Session["id"]);


            var cartItems = _dbContext.Orders
                .Where(o => o.BuyerId == buyerId && o.Status == 0)
                .Include(o => o.OrderMeal)
                .ToList();

            if (cartItems.Any())
            {
                foreach (var cartItem in cartItems)
                {

                    cartItem.Status = 1;


                    cartItem.OrderTime = DateTime.Now;


                    _dbContext.SaveChanges();
                }


            }


            return RedirectToAction("OrderConfirmation");
        }


        public ActionResult OrderConfirmation()
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

            return View();
        }
    }
}