namespace HBM.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using HBM.Web.Models;

    public partial class AddUserRolesWithPermissions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Permissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RolePermissions",
                c => new
                    {
                        Role_Id = c.Int(nullable: false),
                        Permission_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Role_Id, t.Permission_Id })
                .ForeignKey("dbo.UserRoles", t => t.Role_Id, cascadeDelete: true)
                .ForeignKey("dbo.Permissions", t => t.Permission_Id, cascadeDelete: true)
                .Index(t => t.Role_Id)
                .Index(t => t.Permission_Id);
            
            AddColumn("dbo.ApplicationUsers", "UserRoleId", c => c.Int(nullable: true));
            CreateIndex("dbo.ApplicationUsers", "UserRoleId");
            AddForeignKey("dbo.ApplicationUsers", "UserRoleId", "dbo.UserRoles", "Id", cascadeDelete: false);

            foreach (var key in Enum.GetNames(typeof(UserRoleKey)))
            {
                Sql($"insert into UserRoles ([Key]) values ('{key.ToLower()}')");
            }
            foreach (var key in Enum.GetNames(typeof(PermissionKey)))
            {
                Sql($"insert into Permissions ([Key]) values ('{key}')");
            }            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationUsers", "UserRoleId", "dbo.UserRoles");
            DropForeignKey("dbo.RolePermissions", "Permission_Id", "dbo.Permissions");
            DropForeignKey("dbo.RolePermissions", "Role_Id", "dbo.UserRoles");
            DropIndex("dbo.RolePermissions", new[] { "Permission_Id" });
            DropIndex("dbo.RolePermissions", new[] { "Role_Id" });
            DropIndex("dbo.ApplicationUsers", new[] { "UserRoleId" });
            DropColumn("dbo.ApplicationUsers", "UserRoleId");
            DropTable("dbo.RolePermissions");
            DropTable("dbo.Permissions");
            DropTable("dbo.UserRoles");
        }
    }
}
