using CloudPandaNew.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CloudPandaNew.Controllers.API
{
    public class SearchController : ApiController
    {
        CloudPandaEntities _dbContext;

        public SearchController()
        {
            _dbContext = new CloudPandaEntities();
        }

        [HttpGet]
        [Route("api/SearchMeal")]
        public IHttpActionResult SearchMeal(string keyword, string buyerArea)
        {
            if (string.IsNullOrEmpty(buyerArea))
                return BadRequest("Buyer area is required.");

            // Fetch sellers in the specified area
            var sellersByArea = _dbContext.Users
                .Where(x => x.Area == buyerArea && x.Role == "seller")
                .Include(x => x.SellerFranchises.Select(sf => sf.Franchise.Meals))
                .ToList();

            // Find meals whose names contain the keyword as a substring (case-insensitive)
            var meals = sellersByArea.SelectMany(seller => seller.SellerFranchises)
                .SelectMany(franchise => franchise.Franchise.Meals)
                .Where(meal => meal.Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) // Substring matching
                .ToList();

            if (!meals.Any())
                return NotFound();

            return Ok(meals);
        }

    }
}
