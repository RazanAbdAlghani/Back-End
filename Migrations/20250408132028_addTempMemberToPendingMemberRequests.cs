using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace secondVersionFlowSync.Migrations
{
    /// <inheritdoc />
    public partial class addTempMemberToPendingMemberRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TempMember",
                table: "PendingMemberRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TempMember",
                table: "PendingMemberRequests");
        }
    }
}
