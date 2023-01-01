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
    public class CategogiesController : Controller
    {
        private Bonsal_GardentEntities db = new Bonsal_GardentEntities();

        // GET: Admin/Categogies
        public ActionResult Index(string search, int? index)
        {
            var data = db.Categogies;
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
        // GET: Admin/Categogies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Categogy categogy = db.Categogies.Find(id);
            if (categogy == null)
            {
                return HttpNotFound();
            }
            return View(categogy);
        }

        // GET: Admin/Categogies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Categogies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] Categogy categogy)
        {
            if (ModelState.IsValid)
            {
                db.Categogies.Add(categogy);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(categogy);
        }

        // GET: Admin/Categogies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Categogy categogy = db.Categogies.Find(id);
            if (categogy == null)
            {
                return HttpNotFound();
            }
            return View(categogy);
        }

        // POST: Admin/Categogies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] Categogy categogy)
        {
            if (ModelState.IsValid)
            {
                db.Entry(categogy).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(categogy);
        }

        // GET: Admin/Categogies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Categogy categogy = db.Categogies.Find(id);
            if (categogy == null)
            {
                return HttpNotFound();
            }
            return View(categogy);
        }

        // POST: Admin/Categogies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Categogy categogy = db.Categogies.Find(id);
            db.Categogies.Remove(categogy);
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
