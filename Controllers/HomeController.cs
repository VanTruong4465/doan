using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using Bonsal_Gardent.Areas.Admin.Data;
using PagedList;
namespace Bonsal_Gardent.Controllers
{
    public class HomeController : Controller
    {
        Bonsal_GardentEntities db = new Bonsal_GardentEntities();
        public ActionResult Index(int? page)
        {
            return View();
        }
        public ActionResult NewProduct()
        {
            var product = db.Pictures.Include(n => n.Product).Take(3).ToList();
            return View(product);
        }
        [ChildActionOnly]
        public ActionResult SubjectType()
        {
            return PartialView(db.Types.ToList());
        }
        [ChildActionOnly]
        public ActionResult Categories1()
        {
            var id = Request.QueryString["typeid"];
            int typeid = Convert.ToInt32(id);
            /*var Product = db.Products.Include(n => n.Categogy).Where(n=>n.TypeID == typeid);
            Session["TypeID"] = id;*/
            ViewBag.TypeID = typeid;
            var data = db.GetCate(typeid).ToList();
            return PartialView(data);
        }
        public ActionResult TypeShope()
        {
            return PartialView(db.Types.ToList());
        }
        public ActionResult ProductShope(int? id, int? page)
        {
            var typeid = Convert.ToInt32(Request.QueryString["typeid"]);
            int PageNum = (page ?? 1);
            int PageSize = 6;

            foreach (var item in db.Types)
            {
                if (item.ID == id)
                {
                    ViewBag.Name = item.Name.ToString();
                }
            }
            var product = db.Pictures.Include(n => n.Product).Where(n => n.Product.TypeID == typeid).ToList();
            if (id != null)
                product = db.Pictures.Include(n => n.Product).Where(n => n.Product.TypeID == typeid && n.Product.CategoryID == id).ToList();
            return View(product.OrderBy(n => n.ID).ToPagedList(PageNum, PageSize));
        }
        public ActionResult DoSearch(int? id, int? page)
        {

            var productName = Request.QueryString["productName"] == null ? "" : Request.QueryString["productName"].ToString();
            int PageNum = (page ?? 1);
            int PageSize = 6;
            var product = db.Pictures.Include(n => n.Product).Where(n => n.Product.Name.Contains(productName)).ToList();
            ViewBag.KeyWord = productName;
            ViewBag.Count = product.Count();
            return View(product.OrderBy(n => n.ID).ToPagedList(PageNum, PageSize));
        }
        public ActionResult ProductType(int? id, int? page)
        {
            var ID = Request.QueryString["typeid"];
            int typeid = Convert.ToInt32(id);
            int PageNum = (page ?? 1);
            int PageSize = 6;
            foreach (var item in db.Types)
            {
                if (item.ID == id)
                {
                    ViewBag.Name = item.Name.ToString();
                }
            }
            var product = db.Products.Include(n => n.Pictures).Where(n => n.CategoryID == id && n.TypeID == typeid).ToList();
            return View(product.OrderBy(n => n.ID).ToPagedList(PageNum, PageSize));
        }
        public ActionResult ProductDetails(int? id, int? idpath)
        {

            var product = db.Pictures.Include(n => n.Product).Where(n => n.ID == idpath && n.TypeID == id).ToList();
            return View(product);

        }
        public ActionResult Blog(int? page)
        {
            int PageNum = (page ?? 1);
            int PageSize = 6;
            var product = db.Pictures.Include(n => n.Product).ToList();
            return View(product.OrderBy(n => n.ID).ToPagedList(PageNum, PageSize));
        }
        public ActionResult Shope(int? page)
        {
            int PageNum = (page ?? 1);
            int PageSize = 6;
            var product = db.Pictures.Include(n => n.Product).ToList();
            return View(product.OrderBy(n => n.ID).ToPagedList(PageNum, PageSize));
        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult createCommet(CommentProduct std)
        {
            db.CommentProducts.Add(std);
            db.SaveChanges();
            string message = "SUCCESS";
            return Json(new { Message = message, JsonRequestBehavior.AllowGet });


        }
        public ActionResult replyCommet(ReplyComment std)
        {
            db.ReplyComments.Add(std);
            db.SaveChanges();
            string message = "SUCCESS";
            return Json(new { Message = message, JsonRequestBehavior.AllowGet });


        }
        public JsonResult getComment(string id)
        {
            List<CommentProduct> tem = new List<CommentProduct>();
            tem = db.CommentProducts.ToList();
            return Json(tem, JsonRequestBehavior.AllowGet);
        }
    }
}