using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionStock.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Entrepots");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Entrepots",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
