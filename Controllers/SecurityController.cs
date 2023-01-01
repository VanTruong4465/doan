using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bonsal_Gardent.Areas.Admin.Data;
namespace Bonsal_Gardent.Controllers
{
    
    public class SecurityController : Controller
    {
        Bonsal_GardentEntities db = new Bonsal_GardentEntities();
        // GET: Security
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginToSystem(FormCollection frm)
        {
            string Email = frm["CEmail"].ToString();
            string Password = frm["CPassword"].ToString();
            var Result = db.AccCustomers.Where(n => n.Email == Email && n.Password == Password).ToList();
            if(Result.Count() > 0)
            {
                Session["CEmail"] = Email;
                Session["CName"] = Result[0].Name;
                Session["CInfo"] = Result[0];

                Response.Redirect("~/Home/Index");
            }
            if (Result == null)
            {
                ViewBag.Error = "Username or password is invalid";
            }
            ViewBag.Error = "Username or password is invalid";
            return RedirectToAction("Login");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "ID,Name,Password,Email,Phone,Address")] AccCustomer accCustomer)
        {
            if (ModelState.IsValid)
            {
                db.AccCustomers.Add(accCustomer);
                db.SaveChanges();
                return RedirectToAction("Login","Security");
            }

            return RedirectToAction("Register","Security");
        }
    }
}