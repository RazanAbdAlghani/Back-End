using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace secondVersionFlowSync.Migrations
{
    /// <inheritdoc />
    public partial class AddIsApprovedToPendingMemberRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "PendingMemberRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "PendingMemberRequests");
        }
    }
}
