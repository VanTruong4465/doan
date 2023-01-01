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
    public class CommentProductsController : Controller
    {
        private Bonsal_GardentEntities db = new Bonsal_GardentEntities();

        // GET: Admin/CommentProducts
        public ActionResult Index(string search, int? index)
        {
            var commentProducts = db.CommentProducts.Include(c => c.AccCustomer).Include(c => c.Product);
            if (search.IsEmpty())
            {
                return View(commentProducts.ToList().ToPagedList(index ?? 1, 3));
            }
            if (search.IsInt())
            {
                return View(commentProducts.ToList().Where(x => x.ID.Equals(Convert.ToInt32(search))).ToPagedList(index ?? 1, 3));
            }
            return View(commentProducts.ToList().Where(x => x.Product.Name.StartsWith(search)).ToPagedList(index ?? 1, 3));
        }

        // GET: Admin/CommentProducts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentProduct commentProduct = db.CommentProducts.Find(id);
            if (commentProduct == null)
            {
                return HttpNotFound();
            }
            return View(commentProduct);
        }

        // GET: Admin/CommentProducts/Create
        public ActionResult Create()
        {
            ViewBag.AccCustomerID = new SelectList(db.AccCustomers, "ID", "Name");
            ViewBag.AccCustomerID = new SelectList(db.AccCustomers, "ID", "Name");
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name");
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name");
            return View();
        }

        // POST: Admin/CommentProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Content,ProductID,AccCustomerID")] CommentProduct commentProduct)
        {
            if (ModelState.IsValid)
            {
                db.CommentProducts.Add(commentProduct);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccCustomerID = new SelectList(db.AccCustomers, "ID", "Name", commentProduct.AccCustomerID);
            ViewBag.AccCustomerID = new SelectList(db.AccCustomers, "ID", "Name", commentProduct.AccCustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name", commentProduct.ProductID);
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name", commentProduct.ProductID);
            return View(commentProduct);
        }

        // GET: Admin/CommentProducts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentProduct commentProduct = db.CommentProducts.Find(id);
            if (commentProduct == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccCustomerID = new SelectList(db.AccCustomers, "ID", "Name", commentProduct.AccCustomerID);
            ViewBag.AccCustomerID = new SelectList(db.AccCustomers, "ID", "Name", commentProduct.AccCustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name", commentProduct.ProductID);
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name", commentProduct.ProductID);
            return View(commentProduct);
        }

        // POST: Admin/CommentProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Content,ProductID,AccCustomerID")] CommentProduct commentProduct)
        {
            if (ModelState.IsValid)
            {
                db.Entry(commentProduct).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccCustomerID = new SelectList(db.AccCustomers, "ID", "Name", commentProduct.AccCustomerID);
            ViewBag.AccCustomerID = new SelectList(db.AccCustomers, "ID", "Name", commentProduct.AccCustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name", commentProduct.ProductID);
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name", commentProduct.ProductID);
            return View(commentProduct);
        }

        // GET: Admin/CommentProducts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentProduct commentProduct = db.CommentProducts.Find(id);
            if (commentProduct == null)
            {
                return HttpNotFound();
            }
            return View(commentProduct);
        }

        // POST: Admin/CommentProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CommentProduct commentProduct = db.CommentProducts.Find(id);
            db.CommentProducts.Remove(commentProduct);
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
