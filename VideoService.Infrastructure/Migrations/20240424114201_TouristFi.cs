using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoService.Infrastructure.Migrations
{
    public partial class TouristFi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Tourist",
                table: "Tourist");

            migrationBuilder.RenameTable(
                name: "Tourist",
                newName: "Tourists");

            migrationBuilder.AddColumn<int>(
                name: "TouristId",
                table: "Video",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tourists",
                table: "Tourists",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Video_TouristId",
                table: "Video",
                column: "TouristId");

            migrationBuilder.AddForeignKey(
                name: "FK_Video_Tourists_TouristId",
                table: "Video",
                column: "TouristId",
                principalTable: "Tourists",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Video_Tourists_TouristId",
                table: "Video");

            migrationBuilder.DropIndex(
                name: "IX_Video_TouristId",
                table: "Video");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tourists",
                table: "Tourists");

            migrationBuilder.DropColumn(
                name: "TouristId",
                table: "Video");

            migrationBuilder.RenameTable(
                name: "Tourists",
                newName: "Tourist");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tourist",
                table: "Tourist",
                column: "Id");
        }
    }
}
