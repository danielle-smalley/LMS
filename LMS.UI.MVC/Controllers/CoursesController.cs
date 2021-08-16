using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.DATA.EF;

namespace LMS.UI.MVC.Controllers
{
    public class CoursesController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: Courses
        [Authorize(Roles = "Admin, HRAdmin, Manager, Employee")]
        public ActionResult Index()
        {
            var activeCourses = db.Courses.Where(x => x.IsActive == true).ToList();
            return View(activeCourses.ToList());
        }

        [Authorize(Roles = "Admin, HRAdmin")]
        public ActionResult IndexInactiveOnly()
        {
            var inactiveCourses = db.Courses.Where(x => x.IsActive == false).ToList();
            return View(inactiveCourses);
        }

        // GET: Courses/Details/5
        [Authorize(Roles = "Admin, HRAdmin, Manager, Employee")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }

            var lessons = db.Lessons.Where(x => x.CourseId == course.CourseId);
            return View(lessons.ToList());
        }

        //To see course details (have file upload there so need access)
        [Authorize(Roles = "Admin, HRAdmin, Manager, Employee")]
        public ActionResult OriginalDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }

            return View(course);
        }


        // GET: Courses/Create
        [Authorize(Roles = "Admin, HRAdmin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, HRAdmin")]
        public ActionResult Create([Bind(Include = "CourseId,CourseName,CourseDescription,IsActive,CourseImg")] Course course, HttpPostedFileBase courseImg)
        {
            if (ModelState.IsValid)
            {
                string imageName = "noImage.png";
                if (courseImg != null)
                {
                    imageName = courseImg.FileName;

                    string ext = imageName.Substring(imageName.LastIndexOf('.'));

                    string[] goodExts = { ".jpg", ".jpeg", ".png", ".gif", ".jfif" };

                    if (goodExts.Contains(ext.ToLower()))
                    {
                        courseImg.SaveAs(Server.MapPath("~/Content/img/" + imageName));
                    }
                    else
                    {
                        imageName = "noImage.png";
                    }
                }
                course.CourseImg = imageName;




                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Admin, HRAdmin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, HRAdmin")]
        public ActionResult Edit([Bind(Include = "CourseId,CourseName,CourseDescription,IsActive,CourseImg")] Course course, HttpPostedFileBase courseImg)
        {
            if (ModelState.IsValid)
            {
                string imageName = "noImage.png";
                if (courseImg != null)
                {
                    imageName = courseImg.FileName;

                    string ext = imageName.Substring(imageName.LastIndexOf('.'));

                    string[] goodExts = { ".jpg", ".jpeg", ".png", ".gif", ".jfif" };

                    if (goodExts.Contains(ext.ToLower()))
                    {
                        courseImg.SaveAs(Server.MapPath("~/Content/img/" + imageName));
                    }
                    else
                    {
                        imageName = "noImage.png";
                    }
                }
                course.CourseImg = imageName;
            }
            db.Entry(course).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: Courses/Delete/5
        [Authorize(Roles = "Admin, HRAdmin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, HRAdmin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
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
