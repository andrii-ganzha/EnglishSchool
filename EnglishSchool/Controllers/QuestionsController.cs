using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EnglishSchool.Models;
using Microsoft.AspNet.Identity;

namespace EnglishSchool.Controllers
{
    [Authorize]
    public class QuestionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Questions
        //public ActionResult Index()
        //{
        //    var questions = db.Questions.Include(q => q.Quiz).Include(q => q.Tipe);
        //    return View(questions.ToList());
        //}

        // GET: Questions/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Question question = db.Questions.Find(id);
        //    if (question == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(question);
        //}

        // GET: Questions/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.QuizID = new SelectList(db.Quizzes, "QuizID", "Name");
            ViewBag.TipeID = new SelectList(db.Types, "TipeID", "Name");

            ViewBag.ans = db.Answers.ToList();

            return View();
        }


        // POST: Questions/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "QuestionID,Text,TipeID,QuizID")] Question question)
        {
            question.TipeID = 1;
            if (ModelState.IsValid)
            {
                db.Questions.Add(question);
                db.SaveChanges();
                return RedirectToAction("Index", "quizzes", new { id = question.QuizID });
            }

            ViewBag.QuizID = new SelectList(db.Quizzes, "QuizID", "Name", question.QuizID);
            ViewBag.TipeID = new SelectList(db.Types, "TipeID", "Name", question.TipeID);
            return View("Quizzes","Index", new { id = question.QuizID});
        }

        // GET: Questions/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            ViewBag.QuizID = new SelectList(db.Quizzes, "QuizID", "Name", question.QuizID);
            ViewBag.TipeID = new SelectList(db.Types, "TipeID", "Name", question.TipeID);
            return View(question);
        }

        // POST: Questions/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "QuestionID,Text,TipeID,QuizID")] Question question)
        {
            question.TipeID = 1;
            if (ModelState.IsValid)
            {
                db.Entry(question).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "quizzes", new { id = question.QuizID });
            }
            ViewBag.QuizID = new SelectList(db.Quizzes, "QuizID", "Name", question.QuizID);
            ViewBag.TipeID = new SelectList(db.Types, "TipeID", "Name", question.TipeID);
            return View("Quizzes", "Index", new { id = question.QuizID });
        }

        // GET: Questions/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: Questions/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Question question = db.Questions.Find(id);
            db.Questions.Remove(question);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "User")]
        public ActionResult Test(int? QuizID)
        {
            //удаляем предыдущий тест этого пользователя (если он его уже проходил)
            var UserID = User.Identity.GetUserId();
            var q_AllAnswers = db.UserAnswers.Where(a => a.Answer.Question.QuizID == QuizID);
            var allAnswers = q_AllAnswers.ToList();
            var q_Answers = allAnswers.Where(a => a.UserID == UserID);
            var Answers = q_Answers.ToList();
            foreach(var item in Answers)
            {
                db.UserAnswers.Remove(item);
            }
            db.SaveChanges();


            ViewBag.i = 1;
            var questions = db.Questions.Include(q => q.Quiz)
                .Where(q => q.QuizID == QuizID);
            ViewBag.answers = new int[questions.Count()];
            return View(questions.ToList());
        }
        [Authorize(Roles = "User")]
        public string SaveRadioAnswer(int answerID, int questionID)
        {
            UserAnswer answer = new UserAnswer();
            var UserID = User.Identity.GetUserId();
            var q_answers = db.UserAnswers.Where(a => a.Answer.QuestionID == questionID);
            var answers = q_answers.ToList();
            foreach(var item in answers)
            {
                if(item.UserID == UserID)
                {
                    item.Change = false;
                }
            }
            var q_answer = answers.Where(a => a.AnswerID == answerID);
            var r_answer = q_answer.ToList();
            var q2_answer = r_answer.Where(a => a.UserID == UserID);
            var r2_answer = q2_answer.ToList();
            if(r2_answer.Count ==0)
            {
                answer.UserID = UserID;
                answer.AnswerID = answerID;
                answer.Change = true;

                db.UserAnswers.Add(answer);
                db.SaveChanges();
            }
            else
            {
                answer = q_answer.First();
                answer.Change = true;
                db.Entry(answer).State = EntityState.Modified;
                db.SaveChanges();
            }
            return("Answer Saved!");
        }
        [Authorize(Roles = "User")]
        public string SaveAnswer(int answerID)
        {
            UserAnswer answer = new UserAnswer();
            var UserID = User.Identity.GetUserId();
            var q_answer = db.UserAnswers.Where(a => a.AnswerID == answerID);
            var r_answer = q_answer.ToList();
            var q2_answer = r_answer.Where(a => a.UserID == UserID);
            var r2_answer = q2_answer.ToList();
            if (r2_answer.Count == 0)
            {
                answer.UserID = UserID;
                answer.AnswerID = answerID;
                answer.Change = true;

                db.UserAnswers.Add(answer);
                db.SaveChanges();
                return "Answer Saved!";
            }
            else
            {
                answer = q_answer.First();
                if (answer.Change == true)
                    answer.Change = false;
                else
                    answer.Change = true;
                db.Entry(answer).State = EntityState.Modified;
                db.SaveChanges();
                return "Answer Changed!!";
            }
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
