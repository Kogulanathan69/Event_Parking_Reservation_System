using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Event_Parking_Reservation_System.Migrations
{
    public partial class AddPrivateEventBooking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrivateEventBookings",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),

                    UserId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    EventType = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),

                    EventName = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),

                    VenueId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    EventDate = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),

                    GuestCount = table.Column<int>(
                        type: "int",
                        nullable: false),

                    NeedParking = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    ParkingAreaId = table.Column<int>(
                        type: "int",
                        nullable: true),

                    TotalAmount = table.Column<decimal>(
                        type: "decimal(18,2)",
                        nullable: false),

                    Status = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_PrivateEventBookings",
                        x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrivateEventBookings");
        }
    }
}