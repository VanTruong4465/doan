using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Bonsal_Gardent.Areas.Admin.Data;
using PagedList;

namespace Bonsal_Gardent.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private Bonsal_GardentEntities db = new Bonsal_GardentEntities();

        // GET: Admin/Products
        public ActionResult Index(string search, int? index)
        {
            var products = db.Products.Include(p => p.Categogy).Include(p => p.Type);
            if (search.IsEmpty())
            {
                return View(products.ToList().ToPagedList(index ?? 1, 3));
            }
            if (search.IsInt())
            {
                return View(products.ToList().Where(x => x.ID.Equals(Convert.ToInt32(search))).ToPagedList(index ?? 1, 3));
            }
            return View(products.ToList().Where(x => x.Name.StartsWith(search)).ToPagedList(index ?? 1, 3));
        }
        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categogies, "ID", "Name");
            ViewBag.TypeID = new SelectList(db.Types, "ID", "Name");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]       
        public ActionResult Create([Bind(Include = "ID,Name,Info,Place,Address,TypeID,CategoryID,Price")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categogies, "ID", "Name", product.CategoryID);
            ViewBag.TypeID = new SelectList(db.Types, "ID", "Name", product.TypeID);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categogies, "ID", "Name", product.CategoryID);
            ViewBag.TypeID = new SelectList(db.Types, "ID", "Name", product.TypeID);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost,ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Info,Place,Address,TypeID,CategoryID,Price")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categogies, "ID", "Name", product.CategoryID);
            ViewBag.TypeID = new SelectList(db.Types, "ID", "Name", product.TypeID);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
