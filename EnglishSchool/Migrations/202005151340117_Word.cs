namespace EnglishSchool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Word : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Words",
                c => new
                    {
                        WordID = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Translate = c.String(),
                        QuizID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WordID)
                .ForeignKey("dbo.Quizs", t => t.QuizID, cascadeDelete: true)
                .Index(t => t.QuizID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Words", "QuizID", "dbo.Quizs");
            DropIndex("dbo.Words", new[] { "QuizID" });
            DropTable("dbo.Words");
        }
    }
}
