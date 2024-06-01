using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace AutoCab.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddAddresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationLocation",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "StartLocation",
                table: "Trips");

            migrationBuilder.AddColumn<Guid>(
                name: "DestinationAddressId",
                table: "Trips",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "StartAddressId",
                table: "Trips",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AddressLine1 = table.Column<string>(type: "text", nullable: true),
                    AddressLine2 = table.Column<string>(type: "text", nullable: true),
                    AddressLine3 = table.Column<string>(type: "text", nullable: true),
                    AddressLine4 = table.Column<string>(type: "text", nullable: true),
                    TownCity = table.Column<string>(type: "text", nullable: true),
                    Region = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trips_DestinationAddressId",
                table: "Trips",
                column: "DestinationAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_StartAddressId",
                table: "Trips",
                column: "StartAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Addresses_DestinationAddressId",
                table: "Trips",
                column: "DestinationAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Addresses_StartAddressId",
                table: "Trips",
                column: "StartAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Addresses_DestinationAddressId",
                table: "Trips");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Addresses_StartAddressId",
                table: "Trips");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Trips_DestinationAddressId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_StartAddressId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "DestinationAddressId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "StartAddressId",
                table: "Trips");

            migrationBuilder.AddColumn<Point>(
                name: "DestinationLocation",
                table: "Trips",
                type: "geometry (point)",
                nullable: true);

            migrationBuilder.AddColumn<Point>(
                name: "StartLocation",
                table: "Trips",
                type: "geometry (point)",
                nullable: true);
        }
    }
}
