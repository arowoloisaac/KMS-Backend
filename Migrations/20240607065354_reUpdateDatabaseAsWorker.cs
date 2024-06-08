using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Key_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class reUpdateDatabaseAsWorker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestKey_AspNetUsers_WorkerId",
                table: "RequestKey");

            migrationBuilder.DropIndex(
                name: "IX_RequestKey_WorkerId",
                table: "RequestKey");

            migrationBuilder.DropColumn(
                name: "WorkerId",
                table: "RequestKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WorkerId",
                table: "RequestKey",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestKey_WorkerId",
                table: "RequestKey",
                column: "WorkerId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestKey_AspNetUsers_WorkerId",
                table: "RequestKey",
                column: "WorkerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
