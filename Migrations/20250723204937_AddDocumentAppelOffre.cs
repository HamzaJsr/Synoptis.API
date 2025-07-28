using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Synoptis.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentAppelOffre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentsAppelOffre",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NomFichier = table.Column<string>(type: "text", nullable: false),
                    TypeDocument = table.Column<string>(type: "text", nullable: false),
                    DateDepot = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AppelOffreId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeposeParId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentsAppelOffre", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentsAppelOffre_AppelOffres_AppelOffreId",
                        column: x => x.AppelOffreId,
                        principalTable: "AppelOffres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentsAppelOffre_Users_DeposeParId",
                        column: x => x.DeposeParId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsAppelOffre_AppelOffreId",
                table: "DocumentsAppelOffre",
                column: "AppelOffreId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsAppelOffre_DeposeParId",
                table: "DocumentsAppelOffre",
                column: "DeposeParId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentsAppelOffre");
        }
    }
}
