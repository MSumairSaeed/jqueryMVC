using jQueryAjaxUsingAsp.Net.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jQueryAjaxUsingAsp.Net.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ViewAll()
        {
            return View(GetAllEmployee());
        }
        IEnumerable<Employee> GetAllEmployee() 
        {
            using (JqueryAjaxDBEntities db = new JqueryAjaxDBEntities())
            {
                return db.Employees.ToList<Employee>();
            }
        }
        public ActionResult AddUpdate(int id=0)
        {
            Employee emp = new Employee();
            if (id != 0)
            {
                using (JqueryAjaxDBEntities db = new JqueryAjaxDBEntities())
                {
                    emp = db.Employees.Where(x => x.Id == id).FirstOrDefault<Employee>();
                }
            }
            return View(emp);
        }
        [HttpPost]
        public ActionResult AddUpdate(Employee employee)
        {
            try
            {
                if (employee.ImageUpload != null)
                {
                    var fileName = Path.GetFileNameWithoutExtension(employee.ImageUpload.FileName);
                    var extension = Path.GetExtension(employee.ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    employee.ImagePath = "~/AppFiles/Images/" + fileName;
                    employee.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/Images/"), fileName));
                }
                using (JqueryAjaxDBEntities db = new JqueryAjaxDBEntities())
                {
                    if (employee.Id == 0)
                    {
                        db.Employees.Add(employee);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Entry(employee).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }

                //return RedirectToAction("ViewAll");
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllEmployee()), message = "Submitted Successfully" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult Delete(int id)
        {
            try
            {
                using (JqueryAjaxDBEntities db = new JqueryAjaxDBEntities())
                {
                    Employee emp = db.Employees.Where(x => x.Id == id).FirstOrDefault<Employee>();
                    db.Employees.Remove(emp);
                    db.SaveChanges();
                }
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllEmployee()), message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}