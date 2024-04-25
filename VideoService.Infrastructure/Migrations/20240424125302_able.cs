using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoService.Infrastructure.Migrations
{
    public partial class able : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoTouristLike_Tourists_TouristId",
                table: "VideoTouristLike");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoTouristLike_Video_VideoId",
                table: "VideoTouristLike");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoTouristLike",
                table: "VideoTouristLike");

            migrationBuilder.RenameTable(
                name: "VideoTouristLike",
                newName: "TouristLikes");

            migrationBuilder.RenameIndex(
                name: "IX_VideoTouristLike_VideoId",
                table: "TouristLikes",
                newName: "IX_TouristLikes_VideoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TouristLikes",
                table: "TouristLikes",
                columns: new[] { "TouristId", "VideoId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TouristLikes_Tourists_TouristId",
                table: "TouristLikes",
                column: "TouristId",
                principalTable: "Tourists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TouristLikes_Video_VideoId",
                table: "TouristLikes",
                column: "VideoId",
                principalTable: "Video",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TouristLikes_Tourists_TouristId",
                table: "TouristLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_TouristLikes_Video_VideoId",
                table: "TouristLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TouristLikes",
                table: "TouristLikes");

            migrationBuilder.RenameTable(
                name: "TouristLikes",
                newName: "VideoTouristLike");

            migrationBuilder.RenameIndex(
                name: "IX_TouristLikes_VideoId",
                table: "VideoTouristLike",
                newName: "IX_VideoTouristLike_VideoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoTouristLike",
                table: "VideoTouristLike",
                columns: new[] { "TouristId", "VideoId" });

            migrationBuilder.AddForeignKey(
                name: "FK_VideoTouristLike_Tourists_TouristId",
                table: "VideoTouristLike",
                column: "TouristId",
                principalTable: "Tourists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoTouristLike_Video_VideoId",
                table: "VideoTouristLike",
                column: "VideoId",
                principalTable: "Video",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
