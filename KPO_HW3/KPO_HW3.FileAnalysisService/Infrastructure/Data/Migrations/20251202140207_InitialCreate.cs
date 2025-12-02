using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPO_HW3.FileAnalysisService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlagiarismReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPlagiarized = table.Column<bool>(type: "boolean", nullable: false),
                    SimilarityScore = table.Column<double>(type: "double precision", nullable: false),
                    ContentHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Details = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlagiarismReports", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlagiarismReports_AssignmentId_ContentHash",
                table: "PlagiarismReports",
                columns: new[] { "AssignmentId", "ContentHash" });

            migrationBuilder.CreateIndex(
                name: "IX_PlagiarismReports_WorkId",
                table: "PlagiarismReports",
                column: "WorkId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlagiarismReports");
        }
    }
}
