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
    public class WordsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Words
        //public ActionResult Index()
        //{
        //    var words = db.Words.Include(w => w.Quiz);
        //    return View(words.ToList());
        //}

        // GET: Words/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Word word = db.Words.Find(id);
        //    if (word == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(word);
        //}

        // GET: Words/Create
        public ActionResult Create()
        {
            ViewBag.QuizID = new SelectList(db.Quizzes, "QuizID", "Name");
            return View();
        }

        // POST: Words/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WordID,Text,Translate,QuizID")] Word word)
        {
            if (ModelState.IsValid)
            {
                db.Words.Add(word);
                db.SaveChanges();
                return RedirectToAction("../Quizzes/Details", new { id = word.QuizID});
            }

            ViewBag.QuizID = new SelectList(db.Quizzes, "QuizID", "Name", word.QuizID);
            return View(word);
        }

        // GET: Words/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Word word = db.Words.Find(id);
            if (word == null)
            {
                return HttpNotFound();
            }
            ViewBag.QuizID = new SelectList(db.Quizzes, "QuizID", "Name", word.QuizID);
            return View(word);
        }

        // POST: Words/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WordID,Text,Translate,QuizID")] Word word)
        {
            if (ModelState.IsValid)
            {
                db.Entry(word).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("../Quizzes/Details", new { id = word.QuizID });
            }
            ViewBag.QuizID = new SelectList(db.Quizzes, "QuizID", "Name", word.QuizID);
            return View(word);
        }

        // GET: Words/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Word word = db.Words.Find(id);
            if (word == null)
            {
                return HttpNotFound();
            }
            return View(word);
        }

        // POST: Words/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Word word = db.Words.Find(id);
            db.Words.Remove(word);
            db.SaveChanges();
            return RedirectToAction("../Quizzes/Details", new { id = word.QuizID });
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
