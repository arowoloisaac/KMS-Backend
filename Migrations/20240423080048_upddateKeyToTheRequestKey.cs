using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Key_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class upddateKeyToTheRequestKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestKey_Key_KeyId",
                table: "RequestKey");

            migrationBuilder.AlterColumn<Guid>(
                name: "KeyId",
                table: "RequestKey",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestKey_Key_KeyId",
                table: "RequestKey",
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

            migrationBuilder.AlterColumn<Guid>(
                name: "KeyId",
                table: "RequestKey",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestKey_Key_KeyId",
                table: "RequestKey",
                column: "KeyId",
                principalTable: "Key",
                principalColumn: "Id");
        }
    }
}
