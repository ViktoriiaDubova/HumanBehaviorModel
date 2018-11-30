namespace HBM.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddArticlesCommentsDates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Articles", "DatePost", c => c.DateTime(nullable: false));
            AddColumn("dbo.Articles", "DateEdited", c => c.DateTime());
            AddColumn("dbo.Comments", "DatePost", c => c.DateTime(nullable: false));
            AddColumn("dbo.ApplicationUsers", "DateRegistered", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplicationUsers", "DateRegistered");
            DropColumn("dbo.Comments", "DatePost");
            DropColumn("dbo.Articles", "DateEdited");
            DropColumn("dbo.Articles", "DatePost");
        }
    }
}
