using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalGen.Data.Migrations
{
    public partial class AddLegalInformationWithResearchBook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResearchBooks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: false, type: "nvarchar(450)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResearchBooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResearchBooks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LegalInformation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Document = table.Column<string>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    ResearchBookId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LegalInformation_ResearchBooks_ResearchBookId",
                        column: x => x.ResearchBookId,
                        principalTable: "ResearchBooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LegalInformation_ResearchBookId",
                table: "LegalInformation",
                column: "ResearchBookId");

            migrationBuilder.CreateIndex(
                name: "IX_ResearchBooks_UserId",
                table: "ResearchBooks",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LegalInformation");

            migrationBuilder.DropTable(
                name: "ResearchBooks");
        }
    }
}
