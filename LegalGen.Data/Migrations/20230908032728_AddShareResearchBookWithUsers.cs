using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalGen.Data.Migrations
{
    public partial class AddShareResearchBookWithUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResearchBookShares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ResearchBookId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResearchBookShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResearchBookShares_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ResearchBookShares_ResearchBooks_ResearchBookId",
                        column: x => x.ResearchBookId,
                        principalTable: "ResearchBooks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResearchBookShares_ResearchBookId",
                table: "ResearchBookShares",
                column: "ResearchBookId");

            migrationBuilder.CreateIndex(
                name: "IX_ResearchBookShares_UserId",
                table: "ResearchBookShares",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResearchBookShares");
        }
    }
}
