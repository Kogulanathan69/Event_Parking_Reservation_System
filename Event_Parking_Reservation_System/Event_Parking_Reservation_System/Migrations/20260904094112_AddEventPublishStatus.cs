using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Event_Parking_Reservation_System.Migrations
{
    /// <inheritdoc />
    public partial class AddEventPublishStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Events");
        }
    }
}
