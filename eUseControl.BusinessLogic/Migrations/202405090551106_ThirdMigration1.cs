namespace eUseControl.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ThirdMigration1 : DbMigration
    {
        public override void Up()
        {

            AlterColumn("dbo.Products", "Name", c => c.String(nullable: true));
            AlterColumn("dbo.Products", "Description", c => c.String(nullable: true));
            AlterColumn("dbo.Products", "ImagePath", c => c.String(nullable: true));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "ImagePath", c => c.String());
            AlterColumn("dbo.Products", "Description", c => c.String(nullable: true, defaultValue: ""));
            AlterColumn("dbo.Products", "Name", c => c.String());
        }
    }
}
