
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZeroHunger.DB;

namespace server.Controllers
{
    public class RestaurantController : Controller
    {
        public bool SessionIsSet()
        {
            if (Session["RestaurantId"] == null)
            {
                return false;
            }

            return true;
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (Session["RestaurantId"] != null)
            {
                return RedirectToAction("Dashboard");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(Zero_HungerEntities db, string Name)
        {
            var restaurant = (from row in db.Restaurants
                              where row.Name == Name
                              select row).SingleOrDefault();

            if (restaurant != null)
            {
                Session["RestaurantId"] = restaurant.Id;
                return RedirectToAction("Dashboard");
            }

            TempData["loginMsg"] = "User not found";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session["RestaurantId"] = null;
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Dashboard(Zero_HungerEntities db)
        {
            if (Session["RestaurantId"] == null)
            {
                return RedirectToAction("Login");
            }

            var restaurantId = Int32.Parse(Session["RestaurantId"].ToString());

            var restaurant = (from row in db.Restaurants
                              where row.Id == restaurantId
                              select row).FirstOrDefault();

            if (restaurant == null)
            {
                Session["RestaurantId"] = null;
                return RedirectToAction("Dashboard");
            }

            return View(restaurant);
        }

        [HttpGet]
        public ActionResult All(Zero_HungerEntities db)
        {
            var restaurants = db.Restaurants.ToList();
            return View(restaurants);
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Zero_HungerEntities db, Restaurant restaurant)
        {
            db.Restaurants.Add(restaurant);
            db.SaveChanges();
            TempData["addMsg"] = "Restaurant added successfully";
            return RedirectToAction("Add");
        }

        [HttpGet]
        public ActionResult Delete(Zero_HungerEntities db, int id)
        {
            var restaurant = (
                    from row in db.Restaurants
                    where row.Id == id
                    select row
                ).SingleOrDefault();

            if (restaurant != null)
            {
                db.Restaurants.Remove(restaurant);
                db.SaveChanges();
                return RedirectToAction("All");
            }

            return RedirectToAction("All");
        }

        [HttpGet]
        public ActionResult Details(Zero_HungerEntities db, int id)
        {
            var res = (from row in db.Restaurants
                       where row.Id == id
                       select row).SingleOrDefault();
            return View(res);
        }

        [HttpGet]
        public ActionResult Edit(Zero_HungerEntities db, int id)
        {
            var res = (from row in db.Restaurants
                       where row.Id == id
                       select row).SingleOrDefault();
            return View(res);
        }

        [HttpPost]
        public ActionResult Edit(Zero_HungerEntities db, Restaurant restaurant)
        {
            var ext = (from row in db.Restaurants
                       where row.Id == restaurant.Id
                       select row
                ).FirstOrDefault();

            db.Entry(ext).CurrentValues.SetValues(restaurant);
            db.SaveChanges();
            TempData["editMsg"] = "Updated successfully";
            return RedirectToAction("Edit");
        }


    }
}