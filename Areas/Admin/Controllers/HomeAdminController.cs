using Bonsal_Gardent.Areas.Admin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bonsal_Gardent.Areas.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        Bonsal_GardentEntities db = new Bonsal_GardentEntities();

        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LoginAdmin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LogIning(FormCollection form)
        {
            string UserAccount = form["email"].ToString();
            string Password = form["password"].ToString();
            var Result = db.AccManagers.Where(Admin => Admin.Email == UserAccount && Admin.Password == Password).ToList();
            if (Result.Count() > 0)
            {
                Session["Email"] = UserAccount;
                Session["Name"] = Result.ElementAt(0).Name;
                Session["Type"] = "Admin";
                string preURL = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
                
                return Redirect("Index");
            }
            else
            {
                ViewBag.Notice = "Email or Password are not correct !";
                return Redirect("LoginAdmin");
            }
            
        }
    }
}