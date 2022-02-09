using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace RAPITest.Data.Migrations
{
    public partial class ApiSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.CreateTable(
				name: "API",
				columns: table => new
				{
					ApiId = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
					ApiTitle = table.Column<string>(nullable: false),
					UserId = table.Column<string>(maxLength: 450, nullable: false),
					ApiSpecification = table.Column<byte[]>(nullable: false),
					SerializedTests = table.Column<byte[]>(nullable: true),
					TSL = table.Column<byte[]>(nullable: false),
					Dictionary = table.Column<byte[]>(nullable: true),
					NextTest = table.Column<DateTime>(nullable: true),
					TestTimeLoop = table.Column<int>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_API", x => x.ApiId);
					table.ForeignKey(
						name: "FK_CsvFile_AspNetUsers_Id",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Report",
				columns: table => new
				{
					ReportsId = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
					ApiId = table.Column<int>(nullable: false),
					ReportFile = table.Column<byte[]>(nullable: false),
					ReportDate = table.Column<DateTime>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ReportId", x => x.ReportsId);
					table.ForeignKey(
						name: "FK_Report_API",
						column: x => x.ApiId,
						principalTable: "API",
						principalColumn: "ApiId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "ExternalDll",
				columns: table => new
				{
					ExternalDllId = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
					ApiId = table.Column<int>(nullable: false),
					DLL = table.Column<byte[]>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ExternalDllId", x => x.ExternalDllId);
					table.ForeignKey(
						name: "FK_Reports_API",
						column: x => x.ApiId,
						principalTable: "API",
						principalColumn: "ApiId",
						onDelete: ReferentialAction.Cascade);
				});
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.DropTable(
				name: "Report");

			migrationBuilder.DropTable(
				name: "ExternalDll");

			migrationBuilder.DropTable(
				name: "API");
		}
    }
}
