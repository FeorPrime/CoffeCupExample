using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace contracts.Migrations
{
    public partial class SomeChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessingStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentState = table.Column<string>(type: "text", nullable: false),
                    MachineId = table.Column<Guid>(type: "uuid", nullable: false),
                    Step1Done = table.Column<bool>(type: "boolean", nullable: false),
                    Step2Done = table.Column<bool>(type: "boolean", nullable: false),
                    StartStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessingStates", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessingStates");
        }
    }
}
