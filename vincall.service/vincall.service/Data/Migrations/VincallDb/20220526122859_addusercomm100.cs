using Microsoft.EntityFrameworkCore.Migrations;

namespace vincall.service.Data.Migrations.VincallDb
{
    public partial class addusercomm100 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserComm100",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account = table.Column<string>(nullable: true),
                    ExternId = table.Column<string>(maxLength: 200, nullable: true),
                    SiteId = table.Column<string>(nullable: true),
                    PartnerId = table.Column<string>(maxLength: 100, nullable: true),
                    Email = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserComm100", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserComm100");
        }
    }
}
