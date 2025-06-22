using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Synoptis.API.Migrations
{
    /// <inheritdoc />
    public partial class UserHierarchie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ResponsableId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ResponsableId",
                table: "Users",
                column: "ResponsableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_ResponsableId",
                table: "Users",
                column: "ResponsableId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_ResponsableId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ResponsableId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResponsableId",
                table: "Users");
        }
    }
}
