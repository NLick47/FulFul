using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoService.Infrastructure.Migrations
{
    public partial class table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Video_Tourists_TouristId",
                table: "Video");

            migrationBuilder.DropIndex(
                name: "IX_Video_TouristId",
                table: "Video");

            migrationBuilder.DropColumn(
                name: "TouristId",
                table: "Video");

            migrationBuilder.CreateTable(
                name: "VideoTouristLike",
                columns: table => new
                {
                    TouristId = table.Column<int>(type: "int", nullable: false),
                    VideoId = table.Column<int>(type: "int", nullable: false),
                    LikeTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoTouristLike", x => new { x.TouristId, x.VideoId });
                    table.ForeignKey(
                        name: "FK_VideoTouristLike_Tourists_TouristId",
                        column: x => x.TouristId,
                        principalTable: "Tourists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoTouristLike_Video_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Video",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoTouristLike_VideoId",
                table: "VideoTouristLike",
                column: "VideoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoTouristLike");

            migrationBuilder.AddColumn<int>(
                name: "TouristId",
                table: "Video",
                type: "int",
                nullable: true);

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
    }
}
