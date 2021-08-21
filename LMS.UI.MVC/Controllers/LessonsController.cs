using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
            //grabbing the user id
            string userid = User.Identity.GetUserId();

            //looking at lessons the user has viewed (completed), specifically looking at userid
            var completedLessons = db.LessonViews.Where(x => x.UserId == userid).ToList();

            //making sure to just look at active lessons
            var activeLessons = db.Lessons.Where(l => l.IsActive == true);
            //looping through each record in lesson views
            foreach (var item in completedLessons)
            {
                //looping through each COMPLETED lesson in active lessons
                foreach (var cLesson in activeLessons)
                {
                    //marking lessons as complete
                    if (cLesson.LessonId == item.LessonId)
                    {
                        cLesson.hasCompleted = true;
                    }
                    //if we found a match here, this is completed
                    //otherwise, = false
                }
            }

            return View(activeLessons.ToList());
        }

        [Authorize(Roles = "Admin, HRAdmin")]
        public ActionResult IndexInactiveOnly()
        {
            var inactiveLessons = db.Lessons.Where(l => l.IsActive == false);
            return View(inactiveLessons.ToList());
        }

        // GET: Lessons/Details/5 **********************************************************************
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
            //***********************************
            if (lesson != null)
            {
                //grabbing userid
                string userid = User.Identity.GetUserId();

                //creating local variable of type LessonView
                LessonView lv = new LessonView();  //object initialization
                lv.UserId = userid; //doing our get; set; here and below
                lv.LessonId = id;
                lv.DateViewed = DateTime.Now; //one of the requirements is here, by using DateTime.Now to record the time the user completes a lesson. This is also used for courses to mark once a course has been completed, this info sends in an email to the manager.


                //looking for when a lesson is first viewed (hasn't been viewed previously)
                var firstView = db.LessonViews.Where(x => x.LessonId == id && x.UserId == userid).FirstOrDefault();
                if (User.IsInRole("Employee") && firstView == null) //lesson and course completions are only required for employees
                {
                    db.LessonViews.Add(lv);
                    db.SaveChanges(); //adding a lesson view record and updating the DB
                }


                //Below, I'm now comparing the count of active lessons completed vs count of active courses completed
                int coursesToLessonCount = db.Lessons.Where(x => x.CourseId == lesson.CourseId && x.IsActive == true).Count();
                int coursesCompletedCount = db.LessonViews.Where(x => x.Lesson.CourseId == lesson.CourseId && x.UserId == userid && x.Lesson.IsActive == true).Count();

                //if the number of completed lessons in a course is equal to the number of lessons in a course
                if (User.IsInRole("Employee") && coursesToLessonCount == coursesCompletedCount)
                {
                    //creating local variable of class CourseCompletion
                    CourseCompletion completion = new CourseCompletion(); //object initialization
                    completion.UserId = userid;
                    completion.CourseId = lesson.CourseId; //getting; setting; here and above
                    completion.DateCompleted = DateTime.Now;  //needed DateTime.Now here for course completion as well per requirements and handy for the email that sends to manager

                    var firstCompletion = db.CourseCompletions.Where(x => x.UserId == userid && x.CourseId == lesson.CourseId).FirstOrDefault(); //checking to make sure this is the first time completing the course, just like I did with lessons. This is so an employee can't view the same lessons/courses over and over to meet the 6 course completions/year.
                    if (firstCompletion == null)
                    {
                        db.CourseCompletions.Add(completion);
                        db.SaveChanges(); //and course completion record and save changes to DB

                        //building the manager email set up:

                        //finding the full name (partial class I created in the metadata to combine first & last name) of employee
                        string courseTaker = db.UserDetails.Where(x => x.UserId == userid).FirstOrDefault().FullName;

                        //finding course name completed
                        string completedCourse = db.Courses.Where(x => x.CourseId == lesson.CourseId).FirstOrDefault().CourseName;
                        
                        //grabbing date/time course completed
                        var completionDate = completion.DateCompleted;

                        //Below is the message that sends to the manager specifying course, date/time and employee name so the manager can be in the loop
                        string courseCompletedMessage = $"{completedCourse} course was completed at {completionDate:g} by {courseTaker}.";

                       //setting up the email message below, using classes I defined in app secret keys config and referenced in web.config
                        MailMessage mmsg = new MailMessage(ConfigurationManager.AppSettings["EmailUser"].ToString(), ConfigurationManager.AppSettings["EmailTo"].ToString(), "Course Completed", courseCompletedMessage);

                        mmsg.IsBodyHtml = true;
                        mmsg.Priority = MailPriority.High;

                        SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["EmailClient"].ToString());

                        client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["EmailUser"].ToString(), ConfigurationManager.AppSettings["EmailPass"].ToString());

                        //set up a try catch, just in case the email fails to send to manager
                        try
                        {
                            client.Send(mmsg);
                        }
                        catch (Exception ex)
                        {
                            ViewBag.ErrorMessage = $"Oh heck, the course completion notification email could not successfully send to your manager. Please try again later. Error Message: </br> {ex.StackTrace}";
                            throw;
                        }//end catch

                    }//end firstcompletion for course completion
                }//end if courses to complete == courses completed by employee

            }//end lesson viewed

            return View(lesson);
        }//end Details

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
