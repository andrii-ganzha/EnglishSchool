using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EnglishSchool.Models;

namespace EnglishSchool.Controllers
{
    public class UserAnswersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserAnswers
        public ActionResult Index()
        {
            var userAnswers = db.UserAnswers.Include(u => u.Answer).Include(u => u.User);
            return View(userAnswers.ToList());
        }

        // GET: UserAnswers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAnswer userAnswer = db.UserAnswers.Find(id);
            if (userAnswer == null)
            {
                return HttpNotFound();
            }
            return View(userAnswer);
        }

        // GET: UserAnswers/Create
        public ActionResult Create()
        {
            ViewBag.AnswerID = new SelectList(db.Answers, "AnswerID", "Text");
            ViewBag.UserID = new SelectList(db.Users, "Id", "Email");
            return View();
        }

        // POST: UserAnswers/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,AnswerID,Change,k")] UserAnswer userAnswer)
        {
            if (ModelState.IsValid)
            {
                db.UserAnswers.Add(userAnswer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AnswerID = new SelectList(db.Answers, "AnswerID", "Text", userAnswer.AnswerID);
            ViewBag.UserID = new SelectList(db.Users, "Id", "Email", userAnswer.UserID);
            return View(userAnswer);
        }

        // GET: UserAnswers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAnswer userAnswer = db.UserAnswers.Find(id);
            if (userAnswer == null)
            {
                return HttpNotFound();
            }
            ViewBag.AnswerID = new SelectList(db.Answers, "AnswerID", "Text", userAnswer.AnswerID);
            ViewBag.UserID = new SelectList(db.Users, "Id", "Email", userAnswer.UserID);
            return View(userAnswer);
        }

        // POST: UserAnswers/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,AnswerID,Change,k")] UserAnswer userAnswer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userAnswer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AnswerID = new SelectList(db.Answers, "AnswerID", "Text", userAnswer.AnswerID);
            ViewBag.UserID = new SelectList(db.Users, "Id", "Email", userAnswer.UserID);
            return View(userAnswer);
        }

        // GET: UserAnswers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAnswer userAnswer = db.UserAnswers.Find(id);
            if (userAnswer == null)
            {
                return HttpNotFound();
            }
            return View(userAnswer);
        }

        // POST: UserAnswers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            UserAnswer userAnswer = db.UserAnswers.Find(id);
            db.UserAnswers.Remove(userAnswer);
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
