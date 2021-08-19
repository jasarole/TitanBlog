using Microsoft.EntityFrameworkCore.Migrations;

namespace TitanBlog.Data.Migrations
{
    public partial class _006 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostTag_Tag_TagssId",
                table: "PostTag");

            migrationBuilder.RenameColumn(
                name: "TagssId",
                table: "PostTag",
                newName: "TagsId");

            migrationBuilder.RenameIndex(
                name: "IX_PostTag_TagssId",
                table: "PostTag",
                newName: "IX_PostTag_TagsId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostTag_Tag_TagsId",
                table: "PostTag",
                column: "TagsId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostTag_Tag_TagsId",
                table: "PostTag");

            migrationBuilder.RenameColumn(
                name: "TagsId",
                table: "PostTag",
                newName: "TagssId");

            migrationBuilder.RenameIndex(
                name: "IX_PostTag_TagsId",
                table: "PostTag",
                newName: "IX_PostTag_TagssId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostTag_Tag_TagssId",
                table: "PostTag",
                column: "TagssId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
