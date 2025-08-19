using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionStock.Migrations
{
    /// <inheritdoc />
    public partial class newmig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Rayons_RayonId",
                table: "Stocks");

            migrationBuilder.DropTable(
                name: "Rayons");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_RayonId",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "RayonId",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "NombreRayons",
                table: "Entrepots");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RayonId",
                table: "Stocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NombreRayons",
                table: "Entrepots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Rayons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntrepotId = table.Column<int>(type: "int", nullable: false),
                    Capacite = table.Column<int>(type: "int", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rayons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rayons_Entrepots_EntrepotId",
                        column: x => x.EntrepotId,
                        principalTable: "Entrepots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_RayonId",
                table: "Stocks",
                column: "RayonId");

            migrationBuilder.CreateIndex(
                name: "IX_Rayons_EntrepotId",
                table: "Rayons",
                column: "EntrepotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Rayons_RayonId",
                table: "Stocks",
                column: "RayonId",
                principalTable: "Rayons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
