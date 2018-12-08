namespace HBM.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserStats : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserStats",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Rating = c.Int(nullable: false),
                        TimesBanned = c.Int(nullable: false),
                        ArticlesPosted = c.Int(nullable: false),
                        CommentsWritten = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.Id)
                .Index(t => t.Id, unique: true, name: "IX_UserStats");
            
            CreateTable(
                "dbo.UserArticleActivities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ArticleId = c.Int(nullable: false),
                        Viewed = c.Boolean(nullable: false),
                        Vote = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Articles", t => t.ArticleId, cascadeDelete: false)
                .ForeignKey("dbo.ApplicationUsers", t => t.UserId, cascadeDelete: false)
                .Index(t => new { t.UserId, t.ArticleId }, unique: true, name: "IX_UserArticle");
            
            AddColumn("dbo.ApplicationUsers", "FullName", c => c.String(maxLength: 64, unicode: false));
            AddColumn("dbo.ApplicationUsers", "UserStatsId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserArticleActivities", "UserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.UserArticleActivities", "ArticleId", "dbo.Articles");
            DropForeignKey("dbo.UserStats", "Id", "dbo.ApplicationUsers");
            DropIndex("dbo.UserArticleActivities", "IX_UserArticle");
            DropIndex("dbo.UserStats", "IX_UserStats");
            DropColumn("dbo.ApplicationUsers", "UserStatsId");
            DropColumn("dbo.ApplicationUsers", "FullName");
            DropTable("dbo.UserArticleActivities");
            DropTable("dbo.UserStats");
        }
    }
}
