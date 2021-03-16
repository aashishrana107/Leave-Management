using Microsoft.EntityFrameworkCore.Migrations;

namespace leave_management.Data.Migrations
{
    public partial class AddCommentCancelRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CancelRequest",
                table: "LeaveRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CommentRequest",
                table: "LeaveRequests",
                maxLength: 300,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelRequest",
                table: "LeaveRequests");

            migrationBuilder.DropColumn(
                name: "CommentRequest",
                table: "LeaveRequests");
        }
    }
}
