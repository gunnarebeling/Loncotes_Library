using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoncotesLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckoutsToMaterial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Materials_GenreId",
                table: "Materials",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_MaterialTypeId",
                table: "Materials",
                column: "MaterialTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Genres_GenreId",
                table: "Materials",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_MaterialTypes_MaterialTypeId",
                table: "Materials",
                column: "MaterialTypeId",
                principalTable: "MaterialTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_Genres_GenreId",
                table: "Materials");

            migrationBuilder.DropForeignKey(
                name: "FK_Materials_MaterialTypes_MaterialTypeId",
                table: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Materials_GenreId",
                table: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Materials_MaterialTypeId",
                table: "Materials");
        }
    }
}
