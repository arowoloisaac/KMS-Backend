using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Key_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class reUpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestKey_AspNetUsers_WorkerId1",
                table: "RequestKey");

            migrationBuilder.DropIndex(
                name: "IX_RequestKey_WorkerId1",
                table: "RequestKey");

            migrationBuilder.DropColumn(
                name: "WorkerId1",
                table: "RequestKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WorkerId1",
                table: "RequestKey",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestKey_WorkerId1",
                table: "RequestKey",
                column: "WorkerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestKey_AspNetUsers_WorkerId1",
                table: "RequestKey",
                column: "WorkerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
