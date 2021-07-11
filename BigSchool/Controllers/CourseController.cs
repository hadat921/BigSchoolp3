using BigSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BigSchool.Controllers
{
    public class CourseController : Controller
    {
        // GET: Course
        public ActionResult Create()
        {
            BigSchoolContext1 context = new BigSchoolContext1();
            Course objCourse = new Course();
            objCourse.ListCategory = context.Category.ToList();
            return View(objCourse);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course objCourse)
        {
            BigSchoolContext1 context = new BigSchoolContext1();
            ModelState.Remove("LectureId");
            if (!ModelState.IsValid)
            {
                objCourse.ListCategory = context.Category.ToList();
                return View("Create", objCourse);
            }
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objCourse.LectureId = user.Id;
            context.Course.Add(objCourse);
            context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Attending ()
        {
            BigSchoolContext1 context = new BigSchoolContext1();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAttendance = context.Attendance.Where(p => p.Attendee == currentUser.Id).ToList();
            var course = new List<Course>();
            foreach (Attendance temp in listAttendance)
            {
                Course objCourse = temp.Course;
                objCourse.LectureId = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(objCourse.LectureId).Name;
                course.Add(objCourse);
            }
            return View(course);
        }
        public ActionResult Mine ()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext1 context = new BigSchoolContext1();
            var course = context.Course.Where(c => c.LectureId == currentUser.Id && c.DateTime > DateTime.Now ).ToList();
            foreach (Course i in course)
            {
                i.LectureName = currentUser.Name;
            }
            return View(course);
        }
        public ActionResult ListCourse ()
        {
            BigSchoolContext1 context = new BigSchoolContext1();
            var listCourse = context.Course.ToList();
            return View(listCourse);

        }

        public ActionResult Edit(int? id)
        {
            BigSchoolContext1 context = new BigSchoolContext1();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = context.Course.Where(p => p.Id == id && p.LectureId == currentUser.Id).FirstOrDefault();
            if (course == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(context.Category, "Id", "Name", course.CategoryId);
            return View(course);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LectureId,Place,DateTime,CategoryId")] Course course)
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext1 context = new BigSchoolContext1();
            course.LectureId = currentUser.Id;
            ModelState.Remove("LectureId");
            if (ModelState.IsValid)
            {
                context.Entry(course).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Mine");
            }
            ViewBag.CategoryId = new SelectList(context.Category, "Id", "Name", course.CategoryId);
            return View(course);
        }
        public ActionResult Delete(int? id)
        {
            BigSchoolContext1 context = new BigSchoolContext1();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = context.Course.Where(c => c.Id == id && c.LectureId == currentUser.Id).FirstOrDefault();
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCourse(int id)
        {
            BigSchoolContext1 context = new BigSchoolContext1();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Course course = context.Course.Where(c => c.Id == id && c.LectureId == currentUser.Id).FirstOrDefault();
            Attendance attendance = context.Attendance.Where(a => a.CourseId == id).FirstOrDefault();
            context.Course.Remove(course);
            if (attendance != null)
            {
                context.Attendance.Remove(attendance);

            }
            context.SaveChanges();
            return RedirectToAction("Mine");
        }
    }
}