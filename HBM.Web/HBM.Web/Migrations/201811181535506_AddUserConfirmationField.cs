namespace HBM.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserConfirmationField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationUsers", "IsEmailConfirmed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplicationUsers", "IsEmailConfirmed");
        }
    }
}
