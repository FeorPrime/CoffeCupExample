using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace contracts.Migrations
{
    public partial class SomeChanges4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "ProcessingStates",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ProcessingStates");
        }
    }
}
