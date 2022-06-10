using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Vincall.Service.Data.Migrations.VincallDb
{
    public partial class InitialVincallContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Agent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceNumber = table.Column<string>(maxLength: 5, nullable: true),
                    UserAccount = table.Column<string>(maxLength: 20, nullable: true),
                    Remark = table.Column<string>(maxLength: 50, nullable: true),
                    State = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CallList",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExtensionNumber = table.Column<string>(maxLength: 5, nullable: true),
                    From = table.Column<string>(maxLength: 50, nullable: true),
                    To = table.Column<string>(maxLength: 50, nullable: true),
                    CallTime = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CallList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Setting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OptionKey = table.Column<string>(maxLength: 50, nullable: true),
                    OptionValue = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysVersion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Version = table.Column<string>(maxLength: 20, nullable: true),
                    Desc = table.Column<string>(maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysVersion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TwilioSetting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CallId = table.Column<string>(maxLength: 20, nullable: true),
                    AccountSid = table.Column<string>(maxLength: 50, nullable: true),
                    AuthToken = table.Column<string>(maxLength: 50, nullable: true),
                    AppSid = table.Column<string>(maxLength: 50, nullable: true),
                    ApiSid = table.Column<string>(maxLength: 50, nullable: true),
                    ApiSecret = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwilioSetting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account = table.Column<string>(maxLength: 20, nullable: true),
                    Password = table.Column<string>(maxLength: 50, nullable: true),
                    UserName = table.Column<string>(maxLength: 50, nullable: true),
                    Remark = table.Column<string>(maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getutcdate())"),
                    IsAdmin = table.Column<bool>(nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agent");

            migrationBuilder.DropTable(
                name: "CallList");

            migrationBuilder.DropTable(
                name: "Setting");

            migrationBuilder.DropTable(
                name: "SysVersion");

            migrationBuilder.DropTable(
                name: "TwilioSetting");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
