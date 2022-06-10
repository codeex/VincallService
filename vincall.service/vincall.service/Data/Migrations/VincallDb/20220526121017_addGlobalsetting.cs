using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace vincall.service.Data.Migrations.VincallDb
{
    public partial class addGlobalsetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Agent",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getutcdate())");

            migrationBuilder.CreateTable(
                name: "GlobalSetting",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(maxLength: 50, nullable: true),
                    Value = table.Column<string>(maxLength: 8192, nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalSetting", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlobalSetting");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Agent");
        }
    }
}
