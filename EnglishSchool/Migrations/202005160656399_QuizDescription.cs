namespace EnglishSchool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuizDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Quizs", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Quizs", "Description");
        }
    }
}
