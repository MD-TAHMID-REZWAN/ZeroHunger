
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZeroHunger.DB;

namespace server.Controllers
{
    public class RequestController : Controller
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
        public ActionResult All_Restaurant_Request(Zero_HungerEntities db)
        {
            if (!this.SessionIsSet())
            {
                return RedirectToAction("Logout", "Restaurant");
            }

            var restaurantId = Int32.Parse(Session["RestaurantId"].ToString());

            var requests = (from row in db.Requests
                            where row.RestaurantId == restaurantId
                            select row).ToList();

            var PendingRequests = 0;

            foreach (var request in requests)
            {
                if (request.EmployeeId == null) PendingRequests++;
            }

            ViewBag.PendingRequests = PendingRequests;

            return View(requests);
        }

        public ActionResult Delete_Restaurant_Request(Zero_HungerEntities db, int Id)
        {
            if (!this.SessionIsSet())
            {
                return RedirectToAction("Logout", "Restaurant");
            }

            var request = (from row in db.Requests
                           where row.Id == Id
                           select row).SingleOrDefault();

            db.Requests.Remove(request);
            db.SaveChanges();

            return RedirectToAction("All_Restaurant_Request", "Request");
        }

        [HttpGet]
        public ActionResult Open()
        {
            if (this.SessionIsSet() == false)
            {
                return RedirectToAction("Logout", "Restaurant");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Open(Zero_HungerEntities db, Request request, string PreserveUntil)
        {
            DateTime preserveDateTime = DateTime.Parse(PreserveUntil).Date;
            DateTime now = DateTime.Now.Date;

            request.RestaurantId = Int32.Parse(Session["RestaurantId"].ToString());
            request.EmployeeId = null;
            request.IssuedAt = now;
            request.PreserveUntil = preserveDateTime;

            db.Requests.Add(request);
            db.SaveChanges();

            TempData["RequestAddSuccessMsg"] = "Collection request made successfully";

            return RedirectToAction("Open");

        }

        [HttpGet]
        public ActionResult All_Admin_Request(Zero_HungerEntities db)
        {
            var requests = db.Requests.ToList();

            var PendingRequests = 0;

            foreach (var request in requests)
            {
                if (request.EmployeeId == null) PendingRequests++;
            }

            ViewBag.PendingRequests = PendingRequests;

            return View(requests);
        }

        [HttpGet]
        public ActionResult Admin_Request_Details(Zero_HungerEntities db, int Id)
        {
            var request = (from row in db.Requests
                           where row.Id == Id
                           select row).SingleOrDefault();

            return View(request);
        }

        [HttpGet]
        public ActionResult Assign_Employee(Zero_HungerEntities db, int Id)
        {
            var employees = db.Employees.ToList();
            return View(employees);
        }

        [HttpPost]
        public ActionResult Assign_Employee(Zero_HungerEntities db, Request request, Employee employee, int Id, int EmId)
        {
            var req = (from row in db.Requests
                       where row.Id == Id
                       select row).SingleOrDefault();

            request = req;
            request.EmployeeId = EmId;
            db.Entry(req).CurrentValues.SetValues(request);
            db.SaveChanges();

            var emp = (from row in db.Employees
                       where row.Id == EmId
                       select row).SingleOrDefault();

            employee = emp;
            employee.isAvailable = 0;
            db.Entry(emp).CurrentValues.SetValues(employee);
            db.SaveChanges();

            return RedirectToAction("All_Admin_Request");
        }

        [HttpGet]
        public ActionResult Delete(Zero_HungerEntities db, int Id, Employee employee)
        {
            var req = (from row in db.Requests
                       where row.Id == Id
                       select row).SingleOrDefault();

            var EmId = req.EmployeeId;

            db.Requests.Remove(req);

            var emp = (from row in db.Employees
                       where row.Id == EmId
                       select row).SingleOrDefault();

            employee = emp;

            employee.isAvailable = 1;

            db.Entry(emp).CurrentValues.SetValues(employee);

            db.SaveChanges();

            return RedirectToAction("All_Admin_Request");


        }


    }
}