using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Bonsal_Gardent.Areas.Admin.Data;
using PagedList;

namespace Bonsal_Gardent.Areas.Admin.Controllers
{
    public class OrderDetailsController : Controller
    {
        private Bonsal_GardentEntities db = new Bonsal_GardentEntities();

        // GET: Admin/OrderDetails
    
        public ActionResult Index(int ? page)
        {
            var orderDetails = db.OrderDetails.Include(o => o.CustomerOrder).Include(o => o.Product);
            return View(orderDetails.ToList().ToPagedList(page ?? 1, 7));
        }

        // GET: Admin/OrderDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }

        // GET: Admin/OrderDetails/Create
        public ActionResult Create()
        {
            ViewBag.CustomerOrderID = new SelectList(db.CustomerOrders, "ID", "ID");
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name");
            return View();
        }

        // POST: Admin/OrderDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CustomerOrderID,Amount,ProductID,Note")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                db.OrderDetails.Add(orderDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerOrderID = new SelectList(db.CustomerOrders, "ID", "ID", orderDetail.CustomerOrderID);
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name", orderDetail.ProductID);
            return View(orderDetail);
        }

        // GET: Admin/OrderDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerOrderID = new SelectList(db.CustomerOrders, "ID", "ID", orderDetail.CustomerOrderID);
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name", orderDetail.ProductID);
            ViewBag.Status = new SelectList(db.StatusOrders,"ID","Name",orderDetail.Status);
            return View(orderDetail);
        }

        // POST: Admin/OrderDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CustomerOrderID,Price,Quantity,Amount,ProductID,Status,Note")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerOrderID = new SelectList(db.CustomerOrders, "ID", "ID", orderDetail.CustomerOrderID);
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name", orderDetail.ProductID);
            return View(orderDetail);
        }

        // GET: Admin/OrderDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }

        // POST: Admin/OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            db.OrderDetails.Remove(orderDetail);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
