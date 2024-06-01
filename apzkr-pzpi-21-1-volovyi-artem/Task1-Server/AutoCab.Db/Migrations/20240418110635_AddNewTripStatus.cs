using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoCab.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTripStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:car_status", "inactive,idle,en_route,waiting_for_passenger,on_trip,maintenance,danger")
                .Annotation("Npgsql:Enum:trip_status", "created,in_progress,waiting_for_passenger,completed,cancelled")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:Enum:car_status", "inactive,idle,en_route,waiting_for_passenger,on_trip,maintenance,danger")
                .OldAnnotation("Npgsql:Enum:trip_status", "created,in_progress,completed,cancelled")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:car_status", "inactive,idle,en_route,waiting_for_passenger,on_trip,maintenance,danger")
                .Annotation("Npgsql:Enum:trip_status", "created,in_progress,completed,cancelled")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:Enum:car_status", "inactive,idle,en_route,waiting_for_passenger,on_trip,maintenance,danger")
                .OldAnnotation("Npgsql:Enum:trip_status", "created,in_progress,waiting_for_passenger,completed,cancelled")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");
        }
    }
}
