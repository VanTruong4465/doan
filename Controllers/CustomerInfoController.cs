using Bonsal_Gardent.Areas.Admin.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Bonsal_Gardent.Controllers
{
    public class CustomerInfoController : Controller
    {
        // GET: CustomerInfo
        Bonsal_GardentEntities db = new Bonsal_GardentEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _OrderPending()
        {
            AccCustomer cust = (AccCustomer)Session["CInfo"];
            int ID = Convert.ToInt32(cust.ID.ToString());
            var Order = db.OrderDetails.Include(n => n.Product).Include(n => n.CustomerOrder);//.Where(n=>n.CustomerOrder.AccCustomerID.Equals(ID));
            return View(Order.ToList());
        }
        public ActionResult _OrderRejected()
        {
            AccCustomer cust = (AccCustomer)Session["CInfo"];
            int ID = Convert.ToInt32(cust.ID.ToString());
            var Order = db.OrderDetails.Include(n => n.Product).Include(n => n.StatusOrder).Include(n => n.CustomerOrder.AccCustomer.ID.Equals(ID));
            return View(Order.Where(n => n.Status.Equals(2)).ToList());
        }
        public ActionResult _OrderApproved()
        {
            AccCustomer cust = (AccCustomer)Session["CInfo"];
            int ID = Convert.ToInt32(cust.ID.ToString());
            var Order = db.OrderDetails.Include(n => n.Product).Include(n => n.StatusOrder).Include(n => n.CustomerOrder.AccCustomer.ID.Equals(ID));
            return View(Order.Where(n => n.Status.Equals(3)).ToList());
        }
        public ActionResult _OrderReceived()
        {
            AccCustomer cust = (AccCustomer)Session["CInfo"];
            int ID = Convert.ToInt32(cust.ID.ToString());
            var Order = db.OrderDetails.Include(n => n.Product).Include(n => n.StatusOrder).Include(n => n.CustomerOrder.AccCustomer.ID.Equals(ID));
            return View(Order.Where(n => n.Status.Equals(4)).ToList());
        }
        public ActionResult ModifyInfo()
        {
            if (Session["CInfo"] != null)
            {
                int id = Convert.ToInt32(((AccCustomer)Session["CInfo"]).ID.ToString());
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                AccCustomer accCustomer = db.AccCustomers.Find(id);
                if (accCustomer == null)
                {
                    return HttpNotFound();
                }
                return View(accCustomer);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModifyInfo([Bind(Include = "ID,Name,Password,Email,Phone,Address")] AccCustomer accCustomer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accCustomer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(accCustomer);
        }

        public ActionResult ChangePassword(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccCustomer accCustomer = db.AccCustomers.Find(id);
            if (accCustomer == null)
            {
                return HttpNotFound();
            }
            return View(accCustomer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword([Bind(Include = "ID,Password")] AccCustomer accCustomer,FormCollection frm)
        {
            string OldPass = frm["OldPass"];
            string NewPass = frm["NewPass"];
            string pass = frm["Password"];
            if (OldPass == pass)
            {
                if (OldPass != NewPass)
                {
                    if (ModelState.IsValid)
                    {
                        db.Entry(accCustomer).State = EntityState.Modified;   
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return Content("<script language='javascript' type='text/javascript'>alert('New Password look like Old Password!');</script>");
                }
            }
            else
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Old Password must be the same as the current password!');</script>");
            }
            return View(accCustomer);
        }
    }
}