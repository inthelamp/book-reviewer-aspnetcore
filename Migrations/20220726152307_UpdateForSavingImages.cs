using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookReviewer.Migrations
{
    public partial class UpdateForSavingImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_Author_AuthorId",
                table: "Review");

            migrationBuilder.DropTable(
                name: "Author");

            migrationBuilder.DropColumn(
                name: "BookCoverImage",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Image");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Review",
                newName: "ReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_AuthorId",
                table: "Review",
                newName: "IX_Review_ReviewerId");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "Image",
                newName: "FileName");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "Image",
                newName: "Description");

            migrationBuilder.AlterColumn<decimal>(
                name: "Size",
                table: "Image",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<byte[]>(
                name: "Bytes",
                table: "Image",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "Image",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BookCover",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Bytes = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Size = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookCover", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookCover_Review_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Review",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviewer",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviewer", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookCover_ReviewId",
                table: "BookCover",
                column: "ReviewId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Reviewer_ReviewerId",
                table: "Review",
                column: "ReviewerId",
                principalTable: "Reviewer",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_Reviewer_ReviewerId",
                table: "Review");

            migrationBuilder.DropTable(
                name: "BookCover");

            migrationBuilder.DropTable(
                name: "Reviewer");

            migrationBuilder.DropColumn(
                name: "Bytes",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "Image");

            migrationBuilder.RenameColumn(
                name: "ReviewerId",
                table: "Review",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_ReviewerId",
                table: "Review",
                newName: "IX_Review_AuthorId");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Image",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Image",
                newName: "Note");

            migrationBuilder.AddColumn<byte[]>(
                name: "BookCoverImage",
                table: "Review",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Size",
                table: "Image",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Image",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Author",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Author", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Author_AuthorId",
                table: "Review",
                column: "AuthorId",
                principalTable: "Author",
                principalColumn: "Id");
        }
    }
}
