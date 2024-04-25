using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoService.Infrastructure.Migrations
{
    public partial class Tru : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TouristLikes");

            migrationBuilder.DropTable(
                name: "Tourists");

            migrationBuilder.CreateTable(
                name: "TouristLike",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ip = table.Column<int>(type: "int", nullable: false),
                    videoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TouristLike", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TouristLike_videoId",
                table: "TouristLike",
                column: "videoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TouristLike");

            migrationBuilder.CreateTable(
                name: "Tourists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ip = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tourists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TouristLikes",
                columns: table => new
                {
                    TouristId = table.Column<int>(type: "int", nullable: false),
                    VideoId = table.Column<int>(type: "int", nullable: false),
                    LikeTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TouristLikes", x => new { x.TouristId, x.VideoId });
                    table.ForeignKey(
                        name: "FK_TouristLikes_Tourists_TouristId",
                        column: x => x.TouristId,
                        principalTable: "Tourists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TouristLikes_Video_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Video",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TouristLikes_VideoId",
                table: "TouristLikes",
                column: "VideoId");
        }
    }
}
