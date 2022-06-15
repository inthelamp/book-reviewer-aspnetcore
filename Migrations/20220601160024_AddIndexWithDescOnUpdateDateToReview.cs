using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookReviewer.Migrations
{
    public partial class AddIndexWithDescOnUpdateDateToReview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "CREATE NONCLUSTERED INDEX IX_Review_UpdateDate ON Review (UpdateDate DESC)"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex("IX_Review_UpdateDate", "Review");
        }
    }
}
