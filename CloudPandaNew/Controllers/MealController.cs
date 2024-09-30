using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CloudPandaNew.Context;
using System.Data.Entity;


namespace CloudPanda.Controllers
{
    public class MealController : Controller
    {

        CloudPandaEntities _dbContext;

        public MealController()
        {
            _dbContext = new CloudPandaEntities();
        }

        public ActionResult MealPage()
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

            var mealList = _dbContext.Meals.Include(s => s.Franchise).ToList();
            return View(mealList);
        }

        [HttpGet]
        public ActionResult AddMeal()
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

            ViewBag.FranchiseList = new SelectList(_dbContext.Franchises, "Id", "Name");
            return View();
        }



        [HttpPost]
        public ActionResult AddMeal(Meal meal)
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
                if (meal.Id <= 0)
                {
                    _dbContext.Meals.Add(meal);
                    _dbContext.SaveChanges();
                    TempData["MsgAdd"] = "New Meal added successfully";
                }
                else
                {
                    _dbContext.Entry(meal).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    TempData["MsgMod"] = "Meal information modified successfully";
                }

                return RedirectToAction("MealPage");
            }


            ViewBag.FranchiseList = new SelectList(_dbContext.Franchises, "Id", "Name", meal.FranchiseId);
            return View(meal);


        }



            public ActionResult DeleteMeal(int id)
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

            var data = _dbContext.Meals.Where(x => x.Id == id).First();
                _dbContext.Meals.Remove(data);
                _dbContext.SaveChanges();

                return RedirectToAction("MealPage");
            }

            [HttpGet]
            public ActionResult EditMeal(int id)
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


                var meal = _dbContext.Meals.Find(id);
                if (meal == null)
                {
                    return HttpNotFound();
                }


                ViewBag.FranchiseList = new SelectList(_dbContext.Franchises, "Id", "Name", meal.FranchiseId);

                return View(meal);
            }




            [HttpPost]
            public ActionResult CreateMeal(Meal meal)
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


            if (ModelState.IsValid)
                {

                    if (meal.Id == 0)
                    {
                        _dbContext.Meals.Add(meal); 
                    }
                    else
                    {

                        _dbContext.Entry(meal).State = EntityState.Modified; 
                    }


                    _dbContext.SaveChanges();


                    return RedirectToAction("MealPage");
                }


                ViewBag.FranchiseList = new SelectList(_dbContext.Franchises, "Id", "Name", meal.FranchiseId);


                return View(meal); 
            }

        [HttpGet]
        public ActionResult SearchMeal()
        {
            return View();
        }


    }
    }