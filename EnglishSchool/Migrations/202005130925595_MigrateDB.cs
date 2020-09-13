namespace EnglishSchool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserAnswers",
                c => new
                    {
                        UserID = c.String(nullable: false, maxLength: 128),
                        AnswerID = c.Int(nullable: false),
                        Change = c.Boolean(nullable: false),
                        k = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserID, t.AnswerID })
                .ForeignKey("dbo.Answers", t => t.AnswerID, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.AnswerID);
            
            CreateTable(
                "dbo.TestResults",
                c => new
                    {
                        TestResultID = c.Int(nullable: false, identity: true),
                        QuizID = c.Int(nullable: false),
                        UserID = c.String(maxLength: 128),
                        Date = c.DateTime(nullable: false),
                        Mark = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TestResultID)
                .ForeignKey("dbo.Quizs", t => t.QuizID, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.QuizID)
                .Index(t => t.UserID);
            
            AddColumn("dbo.Answers", "TestResult_TestResultID", c => c.Int());
            AddColumn("dbo.Questions", "TestResult_TestResultID", c => c.Int());
            CreateIndex("dbo.Answers", "TestResult_TestResultID");
            CreateIndex("dbo.Questions", "TestResult_TestResultID");
            AddForeignKey("dbo.Answers", "TestResult_TestResultID", "dbo.TestResults", "TestResultID");
            AddForeignKey("dbo.Questions", "TestResult_TestResultID", "dbo.TestResults", "TestResultID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserAnswers", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.TestResults", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.TestResults", "QuizID", "dbo.Quizs");
            DropForeignKey("dbo.Questions", "TestResult_TestResultID", "dbo.TestResults");
            DropForeignKey("dbo.Answers", "TestResult_TestResultID", "dbo.TestResults");
            DropForeignKey("dbo.UserAnswers", "AnswerID", "dbo.Answers");
            DropIndex("dbo.TestResults", new[] { "UserID" });
            DropIndex("dbo.TestResults", new[] { "QuizID" });
            DropIndex("dbo.UserAnswers", new[] { "AnswerID" });
            DropIndex("dbo.UserAnswers", new[] { "UserID" });
            DropIndex("dbo.Questions", new[] { "TestResult_TestResultID" });
            DropIndex("dbo.Answers", new[] { "TestResult_TestResultID" });
            DropColumn("dbo.Questions", "TestResult_TestResultID");
            DropColumn("dbo.Answers", "TestResult_TestResultID");
            DropTable("dbo.TestResults");
            DropTable("dbo.UserAnswers");
        }
    }
}
