namespace EnglishSchool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnswerValue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Answers", "value", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Answers", "value");
        }
    }
}
