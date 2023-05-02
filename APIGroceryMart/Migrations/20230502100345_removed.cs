using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroceryStore.Migrations
{
    public partial class removed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tickets");

            migrationBuilder.DropTable(
                name: "trainDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "trainDetails",
                columns: table => new
                {
                    TrainNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArrivalLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JourneyTime = table.Column<float>(type: "real", nullable: false),
                    SeatCount_AC1tire = table.Column<int>(type: "int", nullable: false),
                    SeatCount_AC2tire = table.Column<int>(type: "int", nullable: false),
                    SeatCount_AC3tire = table.Column<int>(type: "int", nullable: false),
                    SeatCount_SecoundSetting = table.Column<int>(type: "int", nullable: false),
                    SeatCount_Slepper = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrainName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trainDetails", x => x.TrainNo);
                });

            migrationBuilder.CreateTable(
                name: "tickets",
                columns: table => new
                {
                    TicketNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainNo = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "VARCHAR(10)", maxLength: 10, nullable: false),
                    SeatNo = table.Column<string>(type: "VARCHAR(10)", maxLength: 10, nullable: false),
                    UserName = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tickets", x => x.TicketNo);
                    table.ForeignKey(
                        name: "FK_tickets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tickets_trainDetails_TrainNo",
                        column: x => x.TrainNo,
                        principalTable: "trainDetails",
                        principalColumn: "TrainNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tickets_TrainNo",
                table: "tickets",
                column: "TrainNo");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_UserId",
                table: "tickets",
                column: "UserId");
        }
    }
}
