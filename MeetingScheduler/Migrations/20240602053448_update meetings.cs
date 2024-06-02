using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingScheduler.Migrations
{
    /// <inheritdoc />
    public partial class updatemeetings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CreatorId",
                table: "Meetings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "InvitedUserId",
                table: "Meetings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "InvitedUserId",
                table: "Meetings");
        }
    }
}
