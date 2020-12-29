using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class isApprovedProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Photos",
                type: "bit",
                nullable: false,
                defaultValue: false);

        
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Photos");

         
        }
    }
}
