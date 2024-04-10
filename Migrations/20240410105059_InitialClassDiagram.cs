using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Key_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class InitialClassDiagram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Key",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Room = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Key", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestKey",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Activity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestKey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThirdParty",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentHolder = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThirdParty", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Key");

            migrationBuilder.DropTable(
                name: "RequestKey");

            migrationBuilder.DropTable(
                name: "ThirdParty");
        }
    }
}
