using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TitanBlog.Data.Migrations
{
    public partial class _005 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Deleted",
                table: "Comment",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Moderated",
                table: "Comment",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModeratedBody",
                table: "Comment",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModerationReason",
                table: "Comment",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ModeratorId",
                table: "Comment",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ModeratorId",
                table: "Comment",
                column: "ModeratorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_ModeratorId",
                table: "Comment",
                column: "ModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_ModeratorId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_ModeratorId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "Moderated",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "ModeratedBody",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "ModerationReason",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "ModeratorId",
                table: "Comment");
        }
    }
}
