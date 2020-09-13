using EnglishSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnglishSchool.ViewModels
{
    public class QuizIndexData
    {
        public IEnumerable<Quiz> Quizzes { get; set; }
        public IEnumerable<Question> Questions { get; set; }
        public IEnumerable<Answer> Answers { get; set; }
    }
}