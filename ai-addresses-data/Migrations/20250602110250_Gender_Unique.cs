using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ai_addresses_data.Migrations
{
    /// <inheritdoc />
    public partial class Gender_Unique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Genders_Code",
                table: "Genders",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genders_Name",
                table: "Genders",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Genders_Code",
                table: "Genders");

            migrationBuilder.DropIndex(
                name: "IX_Genders_Name",
                table: "Genders");
        }
    }
}
