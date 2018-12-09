namespace HBM.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLockAttribute : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserRoles", "IsLocked", c => c.Boolean(nullable: false));
            AddColumn("dbo.Permissions", "IsLocked", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Permissions", "IsLocked");
            DropColumn("dbo.UserRoles", "IsLocked");
        }
    }
}
