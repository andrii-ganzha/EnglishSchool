using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EnglishSchool.Models
{
    public class Tipe
    {
        public int TipeID { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Question> Questions { get; set; }

    }

    public class Quiz
    {
        public int QuizID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] File { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<TestResult> TestResults { get; set; }
        public virtual ICollection<Word> Words { get; set; }
    }

    public class Word
    {
        public int WordID { get; set; }
        public string Text { get; set; }
        public string Translate { get; set; }
        public int QuizID { get; set; }
        public Quiz Quiz { get; set; }

    }

    public class Question
    {
        public int QuestionID { get; set; }
        public string Text { get; set; }
        public int TipeID { get; set; }
        public int QuizID { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual Tipe Tipe { get; set; }
        public virtual Quiz Quiz { get; set; }
    }

    public class Answer
    {
        public int AnswerID { get; set; }
        public string Text { get; set; }
        public int value { get; set; }
        public bool Correct { get; set; }
        public int QuestionID { get; set; }
        public virtual Question Question { get; set; }

        public virtual ICollection<UserAnswer> UserAnswers { get; set; }
    }

    public class UserAnswer
    {
        [Key]
        [Column(Order = 1)]
        public string UserID { get; set; }
        [Key]
        [Column(Order = 2)]
        public int AnswerID { get; set; }

        public bool Change { get; set; }

        public int k { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Answer Answer { get; set; }
    }

    public class UserSentence
    {
        public string UserID { get; set; }
        public int QuestionID { get; set; }
        public bool Change { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Question Question { get; set; }
    }

    public class TestResult
    {
        public int TestResultID { get; set; }

        public int QuizID { get; set; }

        public string UserID { get; set; }

        public DateTime Date { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }

        public int Mark { get; set; }
        public double MarkPerc { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual Quiz Quiz { get; set; }

        public TestResult()
        {
            Questions = new List<Question>();
            Answers = new List<Answer>();
        }

    }
}