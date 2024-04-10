using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Key_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedClassRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WorkerId",
                table: "RequestKey",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WorkerId",
                table: "Key",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestKey_WorkerId",
                table: "RequestKey",
                column: "WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_Key_WorkerId",
                table: "Key",
                column: "WorkerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Key_AspNetUsers_WorkerId",
                table: "Key",
                column: "WorkerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestKey_AspNetUsers_WorkerId",
                table: "RequestKey",
                column: "WorkerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Key_AspNetUsers_WorkerId",
                table: "Key");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestKey_AspNetUsers_WorkerId",
                table: "RequestKey");

            migrationBuilder.DropIndex(
                name: "IX_RequestKey_WorkerId",
                table: "RequestKey");

            migrationBuilder.DropIndex(
                name: "IX_Key_WorkerId",
                table: "Key");

            migrationBuilder.DropColumn(
                name: "WorkerId",
                table: "RequestKey");

            migrationBuilder.DropColumn(
                name: "WorkerId",
                table: "Key");
        }
    }
}
