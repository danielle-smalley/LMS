using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.DATA.EF;
using Microsoft.AspNet.Identity;

namespace LMS.UI.MVC.Controllers
{
    public class CourseCompletionsController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: CourseCompletions
        [Authorize(Roles = "Admin, HRAdmin, Manager, Employee")]
        public ActionResult Index()
        {
            var courseCompletions = db.CourseCompletions.Include(c => c.Cours).Include(c => c.UserDetail);
            return View(courseCompletions.ToList());
        }

        //Code snippet #1
        //Added this logic and corresponding view so employees can see Course Completion Progress
        public ActionResult EmployeeProgress()
        {
            List<CourseCompletion> empCompletions = new List<CourseCompletion>();
            string userid = User.Identity.GetUserId();
            empCompletions = db.CourseCompletions.Where(x => x.UserId == userid).ToList();
            var courseCompletions = db.CourseCompletions.Include(c => c.Cours).Include(c => c.UserDetail.FullName);
            return View(empCompletions);
        }

        // GET: CourseCompletions/Details/5
        [Authorize(Roles = "Admin, HRAdmin, Manager, Employee")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseCompletion courseCompletion = db.CourseCompletions.Find(id);
            if (courseCompletion == null)
            {
                return HttpNotFound();
            }
            return View(courseCompletion);
        }

        // GET: CourseCompletions/Create
        [Authorize(Roles = "Admin, HRAdmin")]
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName");
            ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "FirstName");
            return View();
        }

        // POST: CourseCompletions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, HRAdmin")]
        public ActionResult Create([Bind(Include = "CourseCompletionId,UserId,CourseId,DateCompleted")] CourseCompletion courseCompletion)
        {
            if (ModelState.IsValid)
            {
                db.CourseCompletions.Add(courseCompletion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", courseCompletion.CourseId);
            ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "FirstName", courseCompletion.UserId);
            return View(courseCompletion);
        }

        // GET: CourseCompletions/Edit/5
        [Authorize(Roles = "Admin, HRAdmin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseCompletion courseCompletion = db.CourseCompletions.Find(id);
            if (courseCompletion == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", courseCompletion.CourseId);
            ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "FirstName", courseCompletion.UserId);
            return View(courseCompletion);
        }

        // POST: CourseCompletions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, HRAdmin")]
        public ActionResult Edit([Bind(Include = "CourseCompletionId,UserId,CourseId,DateCompleted")] CourseCompletion courseCompletion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courseCompletion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", courseCompletion.CourseId);
            ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "FirstName", courseCompletion.UserId);
            return View(courseCompletion);
        }

        // GET: CourseCompletions/Delete/5
        [Authorize(Roles = "Admin, HRAdmin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseCompletion courseCompletion = db.CourseCompletions.Find(id);
            if (courseCompletion == null)
            {
                return HttpNotFound();
            }
            return View(courseCompletion);
        }

        // POST: CourseCompletions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, HRAdmin")]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseCompletion courseCompletion = db.CourseCompletions.Find(id);
            db.CourseCompletions.Remove(courseCompletion);
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
