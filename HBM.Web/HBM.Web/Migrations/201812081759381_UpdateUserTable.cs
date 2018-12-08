namespace HBM.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUserTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationUsers", "About", c => c.String(maxLength: 256));
            AlterColumn("dbo.ApplicationUsers", "FullName", c => c.String(maxLength: 64));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ApplicationUsers", "FullName", c => c.String(maxLength: 64, unicode: false));
            DropColumn("dbo.ApplicationUsers", "About");
        }
    }
}
