using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Synoptis.API.Migrations
{
    /// <inheritdoc />
    public partial class AddStatutToAppelOffre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Statut",
                table: "AppelOffres",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Statut",
                table: "AppelOffres");
        }
    }
}
