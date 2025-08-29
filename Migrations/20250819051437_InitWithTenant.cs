using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Synoptis.API.Migrations
{
    /// <inheritdoc />
    public partial class InitWithTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RaisonSociale = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Siret = table.Column<string>(type: "text", nullable: false),
                    Adresse = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Ville = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CodePostal = table.Column<string>(type: "text", nullable: false),
                    Pays = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    FormeJuridique = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreeLe = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nom = table.Column<string>(type: "text", nullable: false),
                    Prenom = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    MotDePasse = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    CreeLe = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResponsableId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Users_ResponsableId",
                        column: x => x.ResponsableId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppelOffres",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titre = table.Column<string>(type: "text", nullable: false),
                    NomClient = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DateLimite = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Statut = table.Column<int>(type: "integer", nullable: false),
                    CreeLe = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppelOffres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppelOffres_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppelOffres_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentsAppelOffre",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NomFichier = table.Column<string>(type: "text", nullable: false),
                    TypeDocument = table.Column<string>(type: "text", nullable: false),
                    DateDepot = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AppelOffreId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeposeParId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
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
                name: "IX_AppelOffres_CompanyId",
                table: "AppelOffres",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AppelOffres_CreatedById",
                table: "AppelOffres",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsAppelOffre_AppelOffreId",
                table: "DocumentsAppelOffre",
                column: "AppelOffreId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsAppelOffre_DeposeParId",
                table: "DocumentsAppelOffre",
                column: "DeposeParId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CompanyId",
                table: "Users",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ResponsableId",
                table: "Users",
                column: "ResponsableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentsAppelOffre");

            migrationBuilder.DropTable(
                name: "AppelOffres");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
