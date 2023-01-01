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
using Type = Bonsal_Gardent.Areas.Admin.Data.Type;

namespace Bonsal_Gardent.Areas.Admin.Controllers
{
    public class TypesController : Controller
    {
        private Bonsal_GardentEntities db = new Bonsal_GardentEntities();

        // GET: Admin/Types
        public ActionResult Index(string search, int? index)
        {
            var types = db.Types;
            if (search.IsEmpty())
            {
                return View(types.ToList().ToPagedList(index ?? 1, 3));
            }
            if (search.IsInt())
            {
                return View(types.ToList().Where(x => x.ID.Equals(Convert.ToInt32(search))).ToPagedList(index ?? 1, 3));
            }
            return View(types.ToList().Where(x => x.Name.StartsWith(search)).ToPagedList(index ?? 1, 3));
        }

        // GET: Admin/Types/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Type type = db.Types.Find(id);
            if (type == null)
            {
                return HttpNotFound();
            }
            return View(type);
        }

        // GET: Admin/Types/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Types/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] Type type, HttpPostedFileBase[] PostedFile)
        {
            if (ModelState.IsValid)
            {
                string path = "";
                if (PostedFile[0] != null)
                {
                    foreach (HttpPostedFileBase file in PostedFile)
                    {
                        string InputFileName = type.ID + "-Type-" + Path.GetFileName(file.FileName);
                        var ServerSavePath = Path.Combine(Server.MapPath("~/Images/UploadedFiles/Type/") + InputFileName);
                        //Save file to server folder  
                        file.SaveAs(ServerSavePath);
                        path += InputFileName + ",";
                    }
                }
                else
                {
                    path = "Default.jpg,";
                }
                path = path.Remove(path.Length - 1);
                type.Image = path;
                db.Types.Add(type);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(type);
        }

        // GET: Admin/Types/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Type type = db.Types.Find(id);
            if (type == null)
            {
                return HttpNotFound();
            }
            return View(type);
        }

        // POST: Admin/Types/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] Type type, HttpPostedFileBase[] PostedFile)
        {
            if (ModelState.IsValid)
            {
                //Get old paths
                string paths = type.Image;
                //Delete Old Image
                if (paths != null)
                {
                    string[] pathss = paths.Split(',');
                    foreach (var item in pathss)
                    {
                        string fullPath = Request.MapPath("~/Images/UploadedFiles/" + item);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }
                //Modified
                string path = "";
                if (PostedFile[0] != null)
                {
                    foreach (HttpPostedFileBase file in PostedFile)
                    {
                        string InputFileName = type.ID + "-Type-" + Path.GetFileName(file.FileName);
                        var ServerSavePath = Path.Combine(Server.MapPath("~/Images/UploadedFiles/Type/") + InputFileName);
                        //Save file to server folder  
                        file.SaveAs(ServerSavePath);
                        path += InputFileName + ",";
                    }
                }
                else
                {
                    path = "Default.jpg,";
                }
                path = path.Remove(path.Length - 1);
                type.Image = path;
                db.Entry(type).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(type);
        }

        // GET: Admin/Types/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Type type = db.Types.Find(id);
            if (type == null)
            {
                return HttpNotFound();
            }
            return View(type);
        }

        // POST: Admin/Types/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Type type = db.Types.Find(id);
            db.Types.Remove(type);
            string path = type.Image;
            db.SaveChanges();
            if (path != "Default.jpg")
            {
                string[] paths = path.Split(',');
                foreach (var item in paths)
                {
                    string fullPath = Request.MapPath("~/Images/UploadedFiles/Type/" + item);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }
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
