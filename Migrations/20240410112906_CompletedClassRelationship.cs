using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Key_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class CompletedClassRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "KeyCollectorId",
                table: "ThirdParty",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RequestKeyId",
                table: "ThirdParty",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "KeyCollectorId",
                table: "RequestKey",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Faculty",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThirdParty_KeyCollectorId",
                table: "ThirdParty",
                column: "KeyCollectorId");

            migrationBuilder.CreateIndex(
                name: "IX_ThirdParty_RequestKeyId",
                table: "ThirdParty",
                column: "RequestKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestKey_KeyCollectorId",
                table: "RequestKey",
                column: "KeyCollectorId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestKey_AspNetUsers_KeyCollectorId",
                table: "RequestKey",
                column: "KeyCollectorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdParty_AspNetUsers_KeyCollectorId",
                table: "ThirdParty",
                column: "KeyCollectorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdParty_RequestKey_RequestKeyId",
                table: "ThirdParty",
                column: "RequestKeyId",
                principalTable: "RequestKey",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestKey_AspNetUsers_KeyCollectorId",
                table: "RequestKey");

            migrationBuilder.DropForeignKey(
                name: "FK_ThirdParty_AspNetUsers_KeyCollectorId",
                table: "ThirdParty");

            migrationBuilder.DropForeignKey(
                name: "FK_ThirdParty_RequestKey_RequestKeyId",
                table: "ThirdParty");

            migrationBuilder.DropIndex(
                name: "IX_ThirdParty_KeyCollectorId",
                table: "ThirdParty");

            migrationBuilder.DropIndex(
                name: "IX_ThirdParty_RequestKeyId",
                table: "ThirdParty");

            migrationBuilder.DropIndex(
                name: "IX_RequestKey_KeyCollectorId",
                table: "RequestKey");

            migrationBuilder.DropColumn(
                name: "KeyCollectorId",
                table: "ThirdParty");

            migrationBuilder.DropColumn(
                name: "RequestKeyId",
                table: "ThirdParty");

            migrationBuilder.DropColumn(
                name: "KeyCollectorId",
                table: "RequestKey");

            migrationBuilder.DropColumn(
                name: "Faculty",
                table: "AspNetUsers");
        }
    }
}
