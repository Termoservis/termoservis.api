using Microsoft.EntityFrameworkCore.Migrations;

namespace termoservis.api.Migrations
{
    public partial class PlaceCountryMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Country",
                table: "Places",
                newName: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Places_CountryId",
                table: "Places",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Places_Countries_CountryId",
                table: "Places",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Places_Countries_CountryId",
                table: "Places");

            migrationBuilder.DropIndex(
                name: "IX_Places_CountryId",
                table: "Places");

            migrationBuilder.RenameColumn(
                name: "CountryId",
                table: "Places",
                newName: "Country");
        }
    }
}
