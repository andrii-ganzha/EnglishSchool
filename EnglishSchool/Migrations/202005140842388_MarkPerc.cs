namespace EnglishSchool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MarkPerc : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TestResults", "MarkPerc", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TestResults", "MarkPerc");
        }
    }
}
