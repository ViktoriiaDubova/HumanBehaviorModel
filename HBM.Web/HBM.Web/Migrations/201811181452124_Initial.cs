namespace HBM.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Header = c.String(maxLength: 64),
                        Description = c.String(maxLength: 256),
                        Text = c.String(),
                        UserId = c.Int(nullable: false),
                        ImageId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Images", t => t.ImageId)
                .ForeignKey("dbo.ApplicationUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ImageId);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(maxLength: 512),
                        Article_Id = c.Int(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Articles", t => t.Article_Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.User_Id)
                .Index(t => t.Article_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.ApplicationUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 32, unicode: false),
                        Email = c.String(nullable: false, maxLength: 32, unicode: false),
                        PasswordHash = c.String(),
                        ImageId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Images", t => t.ImageId)
                .Index(t => new { t.Username, t.Email }, unique: true, name: "IX_UserIdent")
                .Index(t => t.ImageId);
            
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Path = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(nullable: false, maxLength: 16),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Key, unique: true, name: "IX_TagKey");
            
            CreateTable(
                "dbo.ArticleTags",
                c => new
                    {
                        Article_Id = c.Int(nullable: false),
                        Tag_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Article_Id, t.Tag_Id })
                .ForeignKey("dbo.Articles", t => t.Article_Id, cascadeDelete: true)
                .ForeignKey("dbo.Tags", t => t.Tag_Id, cascadeDelete: true)
                .Index(t => t.Article_Id)
                .Index(t => t.Tag_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Articles", "UserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ArticleTags", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.ArticleTags", "Article_Id", "dbo.Articles");
            DropForeignKey("dbo.Articles", "ImageId", "dbo.Images");
            DropForeignKey("dbo.Comments", "User_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ApplicationUsers", "ImageId", "dbo.Images");
            DropForeignKey("dbo.Comments", "Article_Id", "dbo.Articles");
            DropIndex("dbo.ArticleTags", new[] { "Tag_Id" });
            DropIndex("dbo.ArticleTags", new[] { "Article_Id" });
            DropIndex("dbo.Tags", "IX_TagKey");
            DropIndex("dbo.ApplicationUsers", new[] { "ImageId" });
            DropIndex("dbo.ApplicationUsers", "IX_UserIdent");
            DropIndex("dbo.Comments", new[] { "User_Id" });
            DropIndex("dbo.Comments", new[] { "Article_Id" });
            DropIndex("dbo.Articles", new[] { "ImageId" });
            DropIndex("dbo.Articles", new[] { "UserId" });
            DropTable("dbo.ArticleTags");
            DropTable("dbo.Tags");
            DropTable("dbo.Images");
            DropTable("dbo.ApplicationUsers");
            DropTable("dbo.Comments");
            DropTable("dbo.Articles");
        }
    }
}
