using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Synoptis.API.Migrations
{
    /// <inheritdoc />
    public partial class AddClientsAndContacts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppelOffres_Companies_CompanyId",
                table: "AppelOffres");

            migrationBuilder.DropColumn(
                name: "NomClient",
                table: "AppelOffres");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "AppelOffres",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "ClientId",
                table: "AppelOffres",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    RaisonSociale = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Siret = table.Column<string>(type: "text", nullable: true),
                    Siren = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    TvaIntracom = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Adresse = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Ville = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CodePostal = table.Column<string>(type: "text", nullable: true),
                    Pays = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    SiteWeb = table.Column<string>(type: "text", nullable: true),
                    Telephone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Secteur = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CreeLe = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifieLe = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContactClients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Prenom = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Nom = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Telephone = table.Column<string>(type: "text", nullable: true),
                    Fonction = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Decisionnaire = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactClients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactClients_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContactClients_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppelOffres_ClientId",
                table: "AppelOffres",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CompanyId_RaisonSociale",
                table: "Clients",
                columns: new[] { "CompanyId", "RaisonSociale" });

            migrationBuilder.CreateIndex(
                name: "IX_ContactClients_ClientId",
                table: "ContactClients",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactClients_CompanyId",
                table: "ContactClients",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppelOffres_Clients_ClientId",
                table: "AppelOffres",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppelOffres_Companies_CompanyId",
                table: "AppelOffres",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppelOffres_Clients_ClientId",
                table: "AppelOffres");

            migrationBuilder.DropForeignKey(
                name: "FK_AppelOffres_Companies_CompanyId",
                table: "AppelOffres");

            migrationBuilder.DropTable(
                name: "ContactClients");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_AppelOffres_ClientId",
                table: "AppelOffres");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "AppelOffres");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "AppelOffres",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomClient",
                table: "AppelOffres",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_AppelOffres_Companies_CompanyId",
                table: "AppelOffres",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
