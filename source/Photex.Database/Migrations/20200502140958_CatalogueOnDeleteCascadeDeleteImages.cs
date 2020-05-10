using Microsoft.EntityFrameworkCore.Migrations;

namespace Photex.Database.Migrations
{
    public partial class CatalogueOnDeleteCascadeDeleteImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Catalogues_CatalogueId",
                table: "Images");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Catalogues_CatalogueId",
                table: "Images",
                column: "CatalogueId",
                principalTable: "Catalogues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Catalogues_CatalogueId",
                table: "Images");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Catalogues_CatalogueId",
                table: "Images",
                column: "CatalogueId",
                principalTable: "Catalogues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
