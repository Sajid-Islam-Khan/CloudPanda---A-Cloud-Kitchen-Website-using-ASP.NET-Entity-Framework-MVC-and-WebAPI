using System;
using CloudPandaNew.Context;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;


namespace CloudPanda.Controllers
{
    public class FranchiseController : Controller
    {
        CloudPandaEntities _dbContext;

        public FranchiseController()
        {
            _dbContext = new CloudPandaEntities();
        }
        // GET: Franchise
        public ActionResult FranchisePage()
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


            var franchiseList = _dbContext.Franchises.ToList();

            if (franchiseList == null)
                return HttpNotFound();

            return View(franchiseList);
        }

        [HttpGet]
        public ActionResult AddFranchise(Franchise franchise)
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

            if (franchise.Id > 0)
                return View(franchise);
            else
            {
                ModelState.Clear();
                ViewBag.NoData = 0;
                return View();
            }

        }

        [HttpPost]
        public ActionResult CreateFranchise(Franchise franchise)
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


            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                if (franchise.Id <= 0)
                {
                    _dbContext.Franchises.Add(franchise);
                    _dbContext.SaveChanges();
                    TempData["MsgAdd"] = "New Franchise information added successfully";
                }
                else
                {
                    _dbContext.Entry(franchise).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    TempData["MsgMod"] = "Franchise information modified successfully";
                }

                return RedirectToAction("FranchisePage");

            }

            return View("AddFranchise");

        }

        public ActionResult DeleteFranchise(int id)
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


            var data = _dbContext.Franchises.Where(x => x.Id == id).First();
            _dbContext.Franchises.Remove(data);
            _dbContext.SaveChanges();
            TempData["MsgRem"] = "Franchise information removed successfully";

            return RedirectToAction("FranchisePage");
        }
    }
}