using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EnglishSchool.Models;
using EnglishSchool.ViewModels;
using Microsoft.AspNet.Identity;

namespace EnglishSchool.Controllers
{
    [Authorize]
    public class QuizzesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Quizzes
        public ActionResult Index(int? id, int? QuestionID)
        {
            var viewModel = new QuizIndexData();
            viewModel.Quizzes = db.Quizzes;

            if(User.IsInRole("Admin"))
            {
                if(id!=null)
                    ViewBag.id = id.Value;
                if (QuestionID != null)
                    ViewBag.QuestionID = QuestionID.Value;

                if(id!=null)
                {
                    ViewBag.QuizID = id.Value;
                    viewModel.Questions = viewModel.Quizzes.Where(q => q.QuizID == id.Value).Single().Questions;
                }

                if(QuestionID!= null)
                {
                    ViewBag.QuestionID = QuestionID.Value;
                    var selectedQuestion_a = viewModel.Questions.Where(q => q.QuestionID == QuestionID).Single().Answers;
                    viewModel.Answers = selectedQuestion_a;
                }

            }
            else
            {
                var UserID = System.Web.HttpContext.Current.User.Identity.GetUserId();
                var quizzes = from c in db.Quizzes select c;
                var tr = db.TestResults.Where(t => t.UserID == UserID).OrderByDescending(t=>t.TestResultID);
                viewModel.Quizzes = quizzes.ToList();
                ViewBag.tr = tr.ToList();
                ViewBag.UserID = User.Identity.GetUserId();
            }


            return View(viewModel);
        }

        // GET: Quizzes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quiz quiz = db.Quizzes.Find(id);
            if (quiz == null)
            {
                return HttpNotFound();
            }
            return View(quiz);
        }

        // GET: Quizzes/Create
        [Authorize(Roles ="Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Quizzes/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "QuizID,Name,Description")] Quiz quiz, HttpPostedFileBase uploadHtml)
        {
            if (uploadHtml == null)
            {
                ModelState.AddModelError("File", "Це поле обов'язкове!");
            }
            else
            {
                if ((uploadHtml.ContentType != "text/html") && (uploadHtml.ContentType != "text/html") && (uploadHtml.ContentType != "text/html"))
                {
                    ModelState.AddModelError("File", "Не коректний тип файлу. Підтримуються: *.htm, *.html!");
                }
                if (uploadHtml.ContentLength > 1000000)
                {
                    ModelState.AddModelError("File", "Завеликий файл! Підтримуються файли до 1МБ.");
                }
            }
            if (ModelState.IsValid)
            {
                byte[] fileData = null;
                using (var binaryReader = new BinaryReader(uploadHtml.InputStream))
                {
                    fileData = binaryReader.ReadBytes(uploadHtml.ContentLength);
                }
                quiz.File = fileData;
                db.Quizzes.Add(quiz);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(quiz);
        }

        // GET: Quizzes/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quiz quiz = db.Quizzes.Find(id);
            if (quiz == null)
            {
                return HttpNotFound();
            }
            return View(quiz);
        }

        // POST: Quizzes/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "QuizID,Name,Description")] Quiz quiz, HttpPostedFileBase uploadHtml)
        {
            if (uploadHtml == null)
            {
                ModelState.AddModelError("File", "Це поле обов'язкове!");
            }
            else
            {
                if ((uploadHtml.ContentType != "text/html") && (uploadHtml.ContentType != "text/html") && (uploadHtml.ContentType != "text/html"))
                {
                    ModelState.AddModelError("File", "Не коректний тип файлу. Підтримуються: *.htm, *.html!");
                }
                if (uploadHtml.ContentLength > 1000000)
                {
                    ModelState.AddModelError("File", "Завеликий файл! Підтримуються файли до 1МБ.");
                }
            }
            if (ModelState.IsValid)
            {
                byte[] fileData = null;
                using (var binaryReader = new BinaryReader(uploadHtml.InputStream))
                {
                    fileData = binaryReader.ReadBytes(uploadHtml.ContentLength);
                }
                quiz.File = fileData;
                db.Entry(quiz).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(quiz);
        }

        // GET: Quizzes/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quiz quiz = db.Quizzes.Find(id);
            if (quiz == null)
            {
                return HttpNotFound();
            }
            return View(quiz);
        }

        // POST: Quizzes/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Quiz quiz = db.Quizzes.Find(id);
            db.Quizzes.Remove(quiz);
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
