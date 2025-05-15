using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class IsDeletedFieldAdded3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedBy, UpdatedBy",
                table: "Users",
                newName: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_CreatedBy",
                table: "Books",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Books_UpdatedBy",
                table: "Books",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Users_CreatedBy",
                table: "Books",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Users_UpdatedBy",
                table: "Books",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Users_CreatedBy",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Users_UpdatedBy",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_CreatedBy",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_UpdatedBy",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "CreatedBy, UpdatedBy");
        }
    }
}
