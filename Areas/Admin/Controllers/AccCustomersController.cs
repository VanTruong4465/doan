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
    public class AccCustomersController : Controller
    {
        private Bonsal_GardentEntities db = new Bonsal_GardentEntities();

        // GET: Admin/AccCustomers
        public ActionResult Index(string search, int? index)
        {
            var data = db.AccCustomers;
            if (search.IsEmpty())
            {
                return View(data.ToList().ToPagedList(index ?? 1, 3));
            }
            if (search.IsInt())
            {
                return View(data.ToList().Where(x => x.ID.Equals(Convert.ToInt32(search))).ToPagedList(index ?? 1, 3));
            }
            return View(data.ToList().Where(x => x.Name.StartsWith(search)).ToPagedList(index ?? 1, 3));
        }
        // GET: Admin/AccCustomers/Details/5
        public ActionResult Details(int? id)
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

        // GET: Admin/AccCustomers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/AccCustomers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Password,Email,Phone,Address")] AccCustomer accCustomer)
        {
            if (ModelState.IsValid)
            {
                db.AccCustomers.Add(accCustomer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(accCustomer);
        }

        // GET: Admin/AccCustomers/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Admin/AccCustomers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Password,Email,Phone,Address")] AccCustomer accCustomer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accCustomer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(accCustomer);
        }

        // GET: Admin/AccCustomers/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Admin/AccCustomers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AccCustomer accCustomer = db.AccCustomers.Find(id);
            db.AccCustomers.Remove(accCustomer);
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
