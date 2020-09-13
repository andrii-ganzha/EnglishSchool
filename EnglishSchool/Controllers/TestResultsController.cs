using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;
using EnglishSchool.Models;
using Microsoft.AspNet.Identity;

namespace EnglishSchool.Controllers
{
    [Authorize(Roles = "User")]
    public class TestResultsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TestResults
        [Authorize]
        public ActionResult Index()
        {
            var userID = User.Identity.GetUserId();
            var tests = db.TestResults.Where(t => t.UserID == userID).OrderByDescending(t=>t.TestResultID);
            return View(tests.ToList());
        }

        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestResult test = db.TestResults.Find(id);
            if (test == null)
            {
                return HttpNotFound();
            }
            ViewBag.Mark = test.MarkPerc;
            return View(test);
        }
       // [Authorize(Roles = "Студент")]
        public ActionResult GetResult(int quizID)
        {
            var UserID = User.Identity.GetUserId();
            var q_allAnswers = db.UserAnswers.Where(a => a.Answer.Question.QuizID == quizID);
            var allAnswers = q_allAnswers.ToList();
            var q_Answers = allAnswers.Where(a => a.UserID == UserID);
            var Answers = q_Answers.ToList();
            var test = new TestResult();
            test.QuizID = quizID;
            test.UserID = UserID;
            test.Date = DateTime.UtcNow.AddHours(3);
            int maxmark = db.Answers.Where(a => a.Question.QuizID == quizID).Where(i => i.Correct == true).ToList().Count;
            Dictionary<int, int> tchans = new Dictionary<int, int>();
            Dictionary<int, int> fchans = new Dictionary<int, int>();
            foreach (var item in Answers)
            {
                if (item.Change == true)
                {
                    test.Questions.Add(item.Answer.Question);
                    test.Answers.Add(item.Answer);
                    //test.Mark += item.Answer.value;
                    int anscount = item.Answer.Question.Answers.Where(a => a.Correct).ToList().Count;
                    if (anscount>1)
                    {
                        //double v =  1 / anscount;
                        //double m = 0;
                        //var qans = item.Answer.Question.Answers;
                        //foreach( var a in qans)
                        //{

                        //}
                        if(tchans.ContainsKey(item.Answer.QuestionID) == false)
                        {
                            tchans.Add(item.Answer.QuestionID, 0);
                            fchans.Add(item.Answer.QuestionID, 0);
                        }
                        if (item.Answer.Correct == true)
                        {
                            int v = tchans.Where(c => c.Key == item.Answer.QuestionID).First().Value;
                            v += 1;
                            tchans.Remove(item.Answer.QuestionID);
                            tchans.Add(item.Answer.QuestionID, v);
                        }
                        else
                        {
                            int v = fchans.Where(c => c.Key == item.Answer.QuestionID).First().Value;
                            v += 1;
                            fchans.Remove(item.Answer.QuestionID);
                            fchans.Add(item.Answer.QuestionID, v);
                        }
                    }
                    else
                    {
                        if(item.Answer.Correct==true)
                        {
                            test.Mark++;
                        }
                    }
                }
                db.UserAnswers.Remove(item);
            }
            foreach(var item in tchans)
            {
                int mark = item.Value - fchans.Where(a => a.Key == item.Key).First().Value;
                if (mark < 0)
                    mark = 0;
                test.Mark += mark;
            }
            test.MarkPerc = 100 * test.Mark / maxmark;
            db.TestResults.Add(test);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = test.TestResultID });
        }

        public ActionResult MyStatistic()
        {
            var userID = User.Identity.GetUserId();
            var tests = db.TestResults.Where(t => t.UserID == userID).OrderByDescending(t=>t.QuizID).ToList();
            List<int> usedquiz = new List<int>();
            for(int i=0; i<tests.Count(); i++)
            {
                if(usedquiz.Contains(tests[i].QuizID)==false)
                {
                    usedquiz.Add(tests[i].QuizID);
                }
                else
                {
                    tests.RemoveAt(i);
                    i--;
                }

            }
            tests = tests.Where(t => t.MarkPerc < 75).ToList();
            return View(tests.ToList());
        }


        public FileContentResult GetChart(int? quizID)
        {
            bool full = true;
            var UserID = User.Identity.GetUserId();
            var query = db.TestResults.Where(t=>t.UserID == UserID);
            if (quizID != null)
            {
                query = query.Where(t => t.QuizID == quizID);
                full = false;
            }
            var dates = query.ToList();
            if(dates.Count>5)
            {
                dates.RemoveRange(0, dates.Count - 5);
            }
            var chart = new Chart();
            chart.Width = 700;
            chart.Height = 300;
            chart.BackColor = Color.FromArgb(211, 223, 240);
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BackSecondaryColor = Color.White;
            chart.BackGradientStyle = GradientStyle.TopBottom;
            chart.BorderlineWidth = 1;
            chart.Palette = ChartColorPalette.BrightPastel;
            chart.BorderlineColor = Color.FromArgb(26, 59, 105);
            chart.RenderType = RenderType.BinaryStreaming;
            chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            chart.AntiAliasing = AntiAliasingStyles.All;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;

            chart.Titles.Add(CreateTitle());
            chart.Legends.Add(CreateLegend());
            chart.Series.Add(CreateSeries(dates, full, SeriesChartType.Line, Color.Red));
            chart.ChartAreas.Add(CreateChartArea());

            var ms = new MemoryStream();
            chart.SaveImage(ms);
            return File(ms.GetBuffer(), @"image/png");
        }

        [NonAction]
        //Заголовок
        public Title CreateTitle()
        {
            Title title = new Title();
            title.Text = "Мої останні тестування";
            title.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            title.Font = new Font("Trebuchet MS", 14F, FontStyle.Bold);
            title.ShadowOffset = 3;
            title.ForeColor = Color.FromArgb(26, 59, 105);

            return title;
        }

        [NonAction]
        //Создание серии
        public Series CreateSeries(IList<TestResult> results, bool full,
       SeriesChartType chartType,
       Color color)
        {
            var seriesDetail = new Series();
            seriesDetail.Name = "Результат (%)";
            seriesDetail.IsValueShownAsLabel = false;
            seriesDetail.Color = color;
            seriesDetail.ChartType = chartType;
            seriesDetail.BorderWidth = 2;
            seriesDetail["DrawingStyle"] = "Cylinder";
            seriesDetail["PieDrawingStyle"] = "SoftEdge";
            DataPoint point;

            foreach (var result in results)
            {
                point = new DataPoint();
                if (full == true)
                    point.AxisLabel = result.Quiz.Name;
                else
                    point.AxisLabel = result.Date.ToString();
                point.YValues = new double[] { result.MarkPerc };
                seriesDetail.Points.Add(point);
            }
            seriesDetail.ChartArea = "Result Chart";

            return seriesDetail;
        }

        [NonAction]
        public Legend CreateLegend()
        {
            var legend = new Legend();
            legend.Name = "Результат (%)";
            legend.Docking = Docking.Bottom;
            legend.Alignment = StringAlignment.Center;
            legend.BackColor = Color.Transparent;
            legend.Font = new Font(new FontFamily("Trebuchet MS"), 9);
            legend.LegendStyle = LegendStyle.Row;

            return legend;
        }

        [NonAction]
        public ChartArea CreateChartArea()
        {
            var chartArea = new ChartArea();
            chartArea.Name = "Result Chart";
            chartArea.BackColor = Color.Transparent;
            chartArea.AxisX.IsLabelAutoFit = false;
            chartArea.AxisY.IsLabelAutoFit = false;
            chartArea.AxisX.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.Interval = 1;
            return chartArea;
        }
    


//        // GET: TestResults/Create
//        public ActionResult Create()
//        {
//            ViewBag.UserID = new SelectList(db.Users, "Id", "Email");
//            return View();
//        }

//        // POST: TestResults/Create
//        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
//        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create([Bind(Include = "TestResultID,UserID,Date,Mark")] TestResult testResult)
//        {
//            if (ModelState.IsValid)
//            {
//                db.TestResults.Add(testResult);
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }

//            ViewBag.UserID = new SelectList(db.Users, "Id", "Email", testResult.UserID);
//            return View(testResult);
//        }

//        // GET: TestResults/Edit/5
//        public ActionResult Edit(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            TestResult testResult = db.TestResults.Find(id);
//            if (testResult == null)
//            {
//                return HttpNotFound();
//            }
//            ViewBag.UserID = new SelectList(db.Users, "Id", "Email", testResult.UserID);
//            return View(testResult);
//        }

//        // POST: TestResults/Edit/5
//        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
//        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit([Bind(Include = "TestResultID,UserID,Date,Mark")] TestResult testResult)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Entry(testResult).State = EntityState.Modified;
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }
//            ViewBag.UserID = new SelectList(db.Users, "Id", "Email", testResult.UserID);
//            return View(testResult);
//        }

//        // GET: TestResults/Delete/5
//        public ActionResult Delete(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            TestResult testResult = db.TestResults.Find(id);
//            if (testResult == null)
//            {
//                return HttpNotFound();
//            }
//            return View(testResult);
//        }

//        // POST: TestResults/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(int id)
//        {
//            TestResult testResult = db.TestResults.Find(id);
//            db.TestResults.Remove(testResult);
//            db.SaveChanges();
//            return RedirectToAction("Index");
//        }

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
