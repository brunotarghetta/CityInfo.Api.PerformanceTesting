using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityInfo.Api.Migrations
{
    public partial class NewFieldInPointOfInterestDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "PointsOfInterest",
                newName: "Description");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "PointsOfInterest",
                newName: "Descripcion");
        }
    }
}
