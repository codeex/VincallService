using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace vincall.service.Data.Migrations.VincallDb
{
    public partial class AddDataProtectionKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsAdmin",
                table: "User",
                nullable: false,
                defaultValueSql: "((0))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "((0))");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "CallList",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(getutcdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "(getutcdate())");

            migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FriendlyName = table.Column<string>(nullable: true),
                    Xml = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataProtectionKeys");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAdmin",
                table: "User",
                type: "bit",
                nullable: true,
                defaultValueSql: "((0))",
                oldClrType: typeof(bool),
                oldDefaultValueSql: "((0))");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "CallList",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getutcdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "(getutcdate())");
        }
    }
}
