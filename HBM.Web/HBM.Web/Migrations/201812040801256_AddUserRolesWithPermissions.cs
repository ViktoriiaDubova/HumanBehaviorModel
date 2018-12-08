namespace HBM.Web.Migrations
{
    using System.Data.Entity.Migrations;
    
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

            Sql("insert into UserRoles ([Key]) values ('admin')");
            Sql("insert into UserRoles ([Key]) values ('common')");
            Sql("insert into UserRoles ([Key]) values ('moderator')");
            Sql("insert into UserRoles ([Key]) values ('blocked')");
            Sql("insert into UserRoles ([Key]) values ('unauthorized')");

            Sql("insert into Permissions ([Key]) values ('CreateArticle')");
            Sql("insert into Permissions ([Key]) values ('ReplyArticle')");
            Sql("insert into Permissions ([Key]) values ('DeleteArticle')");
            Sql("insert into Permissions ([Key]) values ('EditArticle')");
            Sql("insert into Permissions ([Key]) values ('DeleteArticleReply')");
            Sql("insert into Permissions ([Key]) values ('BlockUser')");
            Sql("insert into Permissions ([Key]) values ('BlockArticle')");
            Sql("insert into Permissions ([Key]) values ('LogIn')");
            Sql("insert into Permissions ([Key]) values ('AssignRole')");
            Sql("insert into Permissions ([Key]) values ('DeleteOtherUserArticle')");

            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (1,1)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (1,2)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (1,3)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (1,4)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (1,5)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (1,6)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (1,7)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (1,8)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (1,9)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (1,10)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (2,1)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (2,2)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (2,3)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (2,4)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (2,8)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (3,1)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (3,2)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (3,3)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (3,4)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (3,5)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (3,8)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (3,10)");
            Sql("insert into RolePermissions (Role_Id, Permission_Id) values (4,8)");
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
