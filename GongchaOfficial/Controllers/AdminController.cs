using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GongchaOfficial.Models;
using System.IO;
using PagedList;
using PagedList.Mvc;
using System.Data.Entity;

namespace GongchaOfficial.Controllers
{
    public class AdminController : Controller
    {
        dbGongchaDataContext db = new dbGongchaDataContext();
        // GET: Admin
        public ActionResult Index()
        {
            if (Session["AdminAccount"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                return View();
            }
        }

        public ActionResult Product(int ?page)
        {
            if (Session["AdminAccount"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                int pageNumber = (page ?? 1);
                int pageSize = 6;
                return View(db.Products.ToList().OrderBy(n => n.ProductId).ToPagedList(pageNumber, pageSize));
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["AdminAccount"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult Create(Product product, HttpPostedFileBase fileUpload)
        {
            ViewBag.Size = new SelectList(db.Products.ToList().OrderBy(n => n.ProductSize), "Size", "ProductSize");
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            //Them vao CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //Luu ten fie, luu y bo sung thu vien using System.IO;
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //Luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/Content/style/img"), fileName);
                    //Kiem tra hình anh ton tai chua?
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        //Luu hinh anh vao duong dan
                        fileUpload.SaveAs(path);
                    }
                    product.ProductImage = fileName;
                    //Luu vao CSDL
                    db.Products.InsertOnSubmit(product);
                    db.SubmitChanges();
                }
                return RedirectToAction("Product");
            }
        }

        public ActionResult Details(string id)
        {
            if (Session["AdminAccount"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                Product product = db.Products.SingleOrDefault(n => n.ProductId == id);
                ViewBag.ProductId = product.ProductId;
                if (product == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(product);
            }
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            if (Session["AdminAccount"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                //Lay ra doi tuong sach can xoa theo ma
                Product product = db.Products.SingleOrDefault(n => n.ProductId == id);
                ViewBag.ProductId = product.ProductId;
                if (product == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(product);
            }
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult Xacnhanxoa(string id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            Product product = db.Products.SingleOrDefault(n => n.ProductId == id);
            ViewBag.ProductId = product.ProductId;
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.Products.DeleteOnSubmit(product);
            db.SubmitChanges();
            return RedirectToAction("Product");
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (Session["AdminAccount"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                //Lay ra doi tuong sach theo ma
                Product product = db.Products.SingleOrDefault(n => n.ProductId == id);
                ViewBag.ProductId = product.ProductId;
                if (product == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(product);
            }
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult Xacnhansua(string id)
        {
            Product product = db.Products.SingleOrDefault(n => n.ProductId == id);
            ViewBag.ProductId = product.ProductId;
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            UpdateModel(product);
            db.SubmitChanges();
            return RedirectToAction("Product");
        }
    }
}