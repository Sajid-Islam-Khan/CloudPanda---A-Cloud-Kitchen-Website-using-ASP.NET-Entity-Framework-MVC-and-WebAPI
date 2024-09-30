using CloudPandaNew.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudPandaNew.Controllers
{
    public class SellerFranchiseController : Controller
    {
        CloudPandaEntities _dbContext;

        public SellerFranchiseController()
        {
            this._dbContext = new CloudPandaEntities();
        }

        // GET: SellerFranchise
        public ActionResult Index()
        {

            return View();
        }

        [HttpGet]
        public ActionResult EnrollFranchise()
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

            ViewBag.Franchises = new SelectList(_dbContext.Franchises, "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult EnrollFranchise(SellerFranchise sellerFranchise)
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

            if (Session["id"] != null)
            {
                sellerFranchise.SellerId = Convert.ToInt32(Session["id"]);
                sellerFranchise.DateAcquired = DateTime.Now;

                _dbContext.SellerFranchises.Add(sellerFranchise);
                _dbContext.SaveChanges();

                return RedirectToAction("SellerDashboard", "User");
            }

            return View();
        }

        public ActionResult SellerMyFranchises()
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


            var sellerFranchises = _dbContext.SellerFranchises
                                      .Where(sf => sf.SellerId == sellerId)
                                      .Select(sf => sf.Franchise)
                                      .ToList();

            return View(sellerFranchises);
        }
    }
}