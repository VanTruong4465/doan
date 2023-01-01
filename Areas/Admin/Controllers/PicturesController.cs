using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bonsal_Gardent.Areas.Admin.Data;
using PagedList.Mvc;
using PagedList;
using System.Web.WebPages;

namespace Bonsal_Gardent.Areas.Admin.Controllers
{
    public class PicturesController : Controller
    {
        private Bonsal_GardentEntities db = new Bonsal_GardentEntities();

        // GET: Admin/Pictures
        public ActionResult Index(string search,int? index)
        {
            var pictures = db.Pictures.Include(p => p.Product);
            if (search.IsEmpty())
            {
                return View(pictures.ToList().ToPagedList(index ?? 1, 3));
            }
            if(search.IsInt())
            {
                return View(pictures.ToList().Where(x=>x.ID.Equals(Convert.ToInt32(search))).ToPagedList(index ?? 1, 3));
            }
            return View(pictures.ToList().Where(x=>x.Product.Name.StartsWith(search)).ToPagedList(index ?? 1,3));
        }

        // GET: Admin/Pictures/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Picture picture = db.Pictures.Find(id);
            if (picture == null)
            {
                return HttpNotFound();
            }
            return View(picture);
        }

        // GET: Admin/Pictures/Create
        public ActionResult Create()
        {
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name");
            return View();
        }

        // POST: Admin/Pictures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ProductID,Path,TypeID")] Picture picture, HttpPostedFileBase[] PostedFile)
        {
            if (ModelState.IsValid)
            {
                string path = "";
                if (PostedFile[0] != null)
                {
                    foreach (HttpPostedFileBase file in PostedFile)
                    {
                        string InputFileName = picture.ID+"-"+picture.ProductID+"-"+Path.GetFileName(file.FileName);
                        var ServerSavePath = Path.Combine(Server.MapPath("~/Images/UploadedFiles/") + InputFileName);
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
                picture.Path = path;
                db.Pictures.Add(picture);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name", picture.ProductID);
            return View(picture);
        }

        // GET: Admin/Pictures/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Picture picture = db.Pictures.Find(id);
            if (picture == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name", picture.ProductID);
            return View(picture);
        }

        // POST: Admin/Pictures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ProductID,Path,TypeID")] Picture picture, HttpPostedFileBase[] PostedFile)
        {
            if (ModelState.IsValid)
            {
                //Get old paths
                string paths = picture.Path;
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
                        string InputFileName = picture.ID + "-" + picture.ProductID + "-" + Path.GetFileName(file.FileName);
                        var ServerSavePath = Path.Combine(Server.MapPath("~/Images/UploadedFiles/") + InputFileName);
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
                picture.Path = path;
                //Modified Image
                db.Entry(picture).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name", picture.ProductID);
            return View(picture);
        }

        // GET: Admin/Pictures/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Picture picture = db.Pictures.Find(id);
            if (picture == null)
            {
                return HttpNotFound();
            }
            return View(picture);
        }

        // POST: Admin/Pictures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Picture picture = db.Pictures.Find(id);
            db.Pictures.Remove(picture);
            string path = picture.Path;
            db.SaveChanges();
            if (path != "Default.jpg")
            {
                string[] paths = path.Split(',');
                foreach (var item in paths)
                {
                    string fullPath = Request.MapPath("~/Images/UploadedFiles/" + item);
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
