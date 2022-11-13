
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZeroHunger.DB;

namespace server.Controllers
{
    public class EmployeeController : Controller
    {
        [HttpGet]
        public ActionResult All(Zero_HungerEntities db)
        {
            var employees = db.Employees.ToList();
            return View(employees);
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Zero_HungerEntities db, Employee employee, string Name)
        {
            employee.Name = Name;
            employee.isAvailable = 1;
            db.Employees.Add(employee);
            db.SaveChanges();
            return RedirectToAction("All");
        }

        [HttpGet]
        public ActionResult Details(Zero_HungerEntities db, int id)
        {
            var employee = (from row in db.Employees
                            where row.Id == id
                            select row).SingleOrDefault();
            return View(employee);
        }

        [HttpGet]
        public ActionResult Edit(Zero_HungerEntities db, int id)
        {
            var employee = (from row in db.Employees
                            where row.Id == id
                            select row).SingleOrDefault();

            return View(employee);
        }

        [HttpPost]
        public ActionResult Edit(Zero_HungerEntities db, Employee employee, string Name, int Id)
        {


            var employee2 = (from row in db.Employees
                             where row.Id == Id
                             select row).SingleOrDefault();

            employee.Name = Name;
            employee.isAvailable = employee2.isAvailable;

            db.Entry(employee2).CurrentValues.SetValues(employee);
            db.SaveChanges();
            TempData["editMsg"] = "Employee update successful";
            return RedirectToAction("Edit", new { id = employee2.Id });
        }

        [HttpGet]
        public ActionResult Delete(Zero_HungerEntities db, int id)
        {
            var employee = (from row in db.Employees
                            where row.Id == id
                            select row).FirstOrDefault();
            db.Employees.Remove(employee);
            db.SaveChanges();
            return RedirectToAction("All");
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (Session["EmployeeId"] != null)
            {
                return RedirectToAction("Dashboard");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(Zero_HungerEntities db, string Name)
        {
            var employee = (from row in db.Employees
                            where row.Name == Name
                            select row).SingleOrDefault();

            if (employee != null)
            {
                Session["EmployeeId"] = employee.Id;
                return RedirectToAction("Dashboard");
            }

            TempData["loginMsg"] = "User not found";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session["EmployeeId"] = null;
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Dashboard(Zero_HungerEntities db)
        {
            if (Session["EmployeeId"] == null)
            {
                return RedirectToAction("Logout");
            }

            var employeeId = Int32.Parse(Session["EmployeeId"].ToString());

            var employee = (from row in db.Employees
                            where row.Id == employeeId
                            select row).FirstOrDefault();

            if (employee == null)
            {
                Session["EmployeeId"] = null;
                return RedirectToAction("Logout");
            }

            return View(employee);
        }

        [HttpGet]
        public ActionResult Collection_Details(Zero_HungerEntities db, int Id)
        {
            if (Session["EmployeeId"] == null)
            {
                return RedirectToAction("Logout");
            }

            var employee = db.Employees.FirstOrDefault(x => x.Id == Id);

            return View(employee);
        }

        [HttpPost]
        public ActionResult Collection_Details(Zero_HungerEntities db, int Id, int ReqId)
        {
            Employee employee = new Employee();

            var req = db.Requests.FirstOrDefault(x => x.Id == ReqId);

            var emp = db.Employees.FirstOrDefault(x => x.Id == Id);

            employee = emp;

            employee.isAvailable = 1;

            db.Entry(emp).CurrentValues.SetValues(employee);

            db.Requests.Remove(req);

            db.SaveChanges();

            return RedirectToAction("Dashboard");

        }

    }
}