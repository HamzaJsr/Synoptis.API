using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Synoptis.API.Migrations
{
    /// <inheritdoc />
    public partial class AjoutUrlDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "DocumentsAppelOffre",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "DocumentsAppelOffre");
        }
    }
}
