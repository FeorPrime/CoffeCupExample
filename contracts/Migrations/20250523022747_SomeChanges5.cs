using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace contracts.Migrations
{
    public partial class SomeChanges5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ProcessingStates");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "ProcessingStates",
                type: "bytea",
                rowVersion: true,
                nullable: true);
        }
    }
}
