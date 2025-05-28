using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace contracts.Migrations
{
    public partial class SomeChanges3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProcessingStates",
                table: "ProcessingStates");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProcessingStates");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProcessingStates",
                table: "ProcessingStates",
                column: "CorrelationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProcessingStates",
                table: "ProcessingStates");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ProcessingStates",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProcessingStates",
                table: "ProcessingStates",
                column: "Id");
        }
    }
}
