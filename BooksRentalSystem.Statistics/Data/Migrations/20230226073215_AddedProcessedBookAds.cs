using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BooksRentalSystem.Statistics.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedProcessedBookAds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessedBookAds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessedBookAds", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessedBookAds");
        }
    }
}
