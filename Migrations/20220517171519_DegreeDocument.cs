using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class DegreeDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DisabilityOfDegreeDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    DisabilityDegreeTypeId = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidAlways = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisabilityOfDegreeDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisabilityOfDegreeDocuments_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DisabilityOfDegreeDocuments_DisabilityDegreeTypes_DisabilityDegreeTypeId",
                        column: x => x.DisabilityDegreeTypeId,
                        principalTable: "DisabilityDegreeTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentificationDocumentSymbols",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisabilityOfDegreeDocumentId = table.Column<int>(type: "int", nullable: false),
                    DisabilityDegreeDictId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentificationDocumentSymbols", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentificationDocumentSymbols_DisabilityDegreeSymbols_DisabilityDegreeDictId",
                        column: x => x.DisabilityDegreeDictId,
                        principalTable: "DisabilityDegreeSymbols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdentificationDocumentSymbols_DisabilityOfDegreeDocuments_DisabilityOfDegreeDocumentId",
                        column: x => x.DisabilityOfDegreeDocumentId,
                        principalTable: "DisabilityOfDegreeDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DisabilityOfDegreeDocuments_DisabilityDegreeTypeId",
                table: "DisabilityOfDegreeDocuments",
                column: "DisabilityDegreeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DisabilityOfDegreeDocuments_OwnerId",
                table: "DisabilityOfDegreeDocuments",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentificationDocumentSymbols_DisabilityDegreeDictId",
                table: "IdentificationDocumentSymbols",
                column: "DisabilityDegreeDictId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentificationDocumentSymbols_DisabilityOfDegreeDocumentId",
                table: "IdentificationDocumentSymbols",
                column: "DisabilityOfDegreeDocumentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentificationDocumentSymbols");

            migrationBuilder.DropTable(
                name: "DisabilityOfDegreeDocuments");
        }
    }
}
