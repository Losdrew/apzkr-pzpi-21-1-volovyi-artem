using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoCab.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDoorOpen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDoorOpen",
                table: "Cars",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDoorOpen",
                table: "Cars");
        }
    }
}
