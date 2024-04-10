using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Key_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AddKeyConnectionsToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "KeyId",
                table: "ThirdParty",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "KeyId",
                table: "RequestKey",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ThirdParty_KeyId",
                table: "ThirdParty",
                column: "KeyId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestKey_KeyId",
                table: "RequestKey",
                column: "KeyId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestKey_Key_KeyId",
                table: "RequestKey",
                column: "KeyId",
                principalTable: "Key",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdParty_Key_KeyId",
                table: "ThirdParty",
                column: "KeyId",
                principalTable: "Key",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestKey_Key_KeyId",
                table: "RequestKey");

            migrationBuilder.DropForeignKey(
                name: "FK_ThirdParty_Key_KeyId",
                table: "ThirdParty");

            migrationBuilder.DropIndex(
                name: "IX_ThirdParty_KeyId",
                table: "ThirdParty");

            migrationBuilder.DropIndex(
                name: "IX_RequestKey_KeyId",
                table: "RequestKey");

            migrationBuilder.DropColumn(
                name: "KeyId",
                table: "ThirdParty");

            migrationBuilder.DropColumn(
                name: "KeyId",
                table: "RequestKey");
        }
    }
}
