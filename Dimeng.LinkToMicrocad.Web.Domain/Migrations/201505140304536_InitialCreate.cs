namespace Dimeng.LinkToMicrocad.Web.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Materials",
                c => new
                    {
                        MaterialId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        TextureId = c.Int(),
                    })
                .PrimaryKey(t => t.MaterialId)
                .ForeignKey("dbo.Textures", t => t.TextureId)
                .Index(t => t.TextureId);
            
            CreateTable(
                "dbo.Textures",
                c => new
                    {
                        TextureId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ImageName = c.String(),
                    })
                .PrimaryKey(t => t.TextureId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Category = c.String(),
                        Width = c.Double(nullable: false),
                        Height = c.Double(nullable: false),
                        Depth = c.Double(nullable: false),
                        Elevation = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Materials", "TextureId", "dbo.Textures");
            DropIndex("dbo.Materials", new[] { "TextureId" });
            DropTable("dbo.Products");
            DropTable("dbo.Textures");
            DropTable("dbo.Materials");
        }
    }
}
