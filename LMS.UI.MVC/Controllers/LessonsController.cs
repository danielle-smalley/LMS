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
    public class LessonsController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: Lessons
        [Authorize(Roles = "Admin, HRAdmin, Manager, Employee")]
        public ActionResult Index()
        {
            var lessons = db.Lessons.Include(l => l.Cours);
            return View(lessons.ToList());
        }

        // GET: Lessons/Details/5
        [Authorize(Roles = "Admin, HRAdmin, Manager, Employee")]
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }

            if (lesson != null)
            {
                string userid = User.Identity.GetUserId();
                LessonView lv = new LessonView();
                lv.UserId = userid;
                lv.LessonId = id;
                lv.DateViewed = DateTime.Now;


                var firstView = db.LessonViews.Where(x => x.LessonId == id && x.UserId == userid).FirstOrDefault();
                if (User.IsInRole("Employee") && firstView == null)
                {
                    db.LessonViews.Add(lv);
                    db.SaveChanges();
                }
            }

            return View(lesson);
        }

        // GET: Lessons/Create
        [Authorize(Roles = "Admin, HRAdmin")]
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName");
            return View();
        }

        // POST: Lessons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, HRAdmin")]
        public ActionResult Create([Bind(Include = "LessonId,LessonTitle,CourseId,LessonDesc,VideoURL,PdfFile,IsActive")] Lesson lesson, HttpPostedFileBase pdfFile)
        {

            if (lesson.VideoURL != null)
            {
                if (lesson.VideoURL.Contains("/watch"))
                {
                    lesson.VideoURL = lesson.VideoURL.Replace("/watch?v=", "/embed/");
                }

            }


                if (ModelState.IsValid)
                {
                    //establish a variable for our default image
                    string fileName = "noPdf.pdf";
                    //if a file was sent
                    if (pdfFile != null)
                    {
                        //reassign the variable to the filename sent over
                        fileName = pdfFile.FileName;

                        //create a variable for pdf extension
                        string ext = fileName.Substring(fileName.LastIndexOf('.'));

                        //create list of extensions
                        string[] goodExts = { ".pdf" };

                        //if ext is valid, assign a GUID as the name and concatonate ext
                        if (goodExts.Contains(ext.ToLower()))
                        {
                            pdfFile.SaveAs(Server.MapPath("~/Content/img/pdf/" + fileName));
                        }//end inner if
                        else
                        {
                            fileName = "noPdf.pdf";
                        }//end else

                    }//end outer if

                    lesson.PdfFile = fileName;

                    db.Lessons.Add(lesson);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
                return View(lesson);
        }


        // GET: Lessons/Edit/5
        [Authorize(Roles = "Admin, HRAdmin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, HRAdmin")]
        public ActionResult Edit([Bind(Include = "LessonId,LessonTitle,CourseId,LessonDesc,VideoURL,PdfFile,IsActive")] Lesson lesson, HttpPostedFileBase pdfFile)
        {

            if (lesson.VideoURL != null)
            {
                if (lesson.VideoURL.Contains("/watch"))
                {
                    lesson.VideoURL = lesson.VideoURL.Replace("/watch?v=", "/embed/");
                }
            }



            if (ModelState.IsValid)
            {
                //establish a variable for our default image
                string fileName = "noPdf.pdf";
                //if a file was sent
                if (pdfFile != null)
                {
                    //reassign the variable to the filename sent over
                    fileName = pdfFile.FileName;

                    //create a variable for pdf extension
                    string ext = fileName.Substring(fileName.LastIndexOf('.'));

                    //create list of extensions
                    string[] goodExts = { ".pdf" };

                    //if ext is valid, assign a GUID as the name and concatonate ext
                    if (goodExts.Contains(ext.ToLower()))
                    {
                        pdfFile.SaveAs(Server.MapPath("~/Content/img/pdf/" + fileName));
                    }//end inner if
                    else
                    {
                        fileName = "noPdf.pdf";
                    }//end else

                }//end outer if

                lesson.PdfFile = fileName;



                db.Entry(lesson).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        // GET: Lessons/Delete/5
        [Authorize(Roles = "Admin, HRAdmin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }

        // POST: Lessons/Delete/5
        [Authorize(Roles = "Admin, HRAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Lesson lesson = db.Lessons.Find(id);
            db.Lessons.Remove(lesson);
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
