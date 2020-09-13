namespace EnglishSchool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuizFile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Quizs", "File", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Quizs", "File");
        }
    }
}
