using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.API.Data.Migrations
{
    public partial class plateStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReserved",
                table: "Plates");

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Plates",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "PlateStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlateStatus", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "PlateStatus",
                columns: new[] { "Id", "Description" },
                values: new object[] { 1, "Plate is available for purchase" });

            migrationBuilder.InsertData(
                table: "PlateStatus",
                columns: new[] { "Id", "Description" },
                values: new object[] { 2, "Plate is temporarily reserved" });

            migrationBuilder.InsertData(
                table: "PlateStatus",
                columns: new[] { "Id", "Description" },
                values: new object[] { 3, "Plate has been sold" });

            migrationBuilder.CreateIndex(
                name: "IX_Plates_StatusId",
                table: "Plates",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Plates_PlateStatus_StatusId",
                table: "Plates",
                column: "StatusId",
                principalTable: "PlateStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plates_PlateStatus_StatusId",
                table: "Plates");

            migrationBuilder.DropTable(
                name: "PlateStatus");

            migrationBuilder.DropIndex(
                name: "IX_Plates_StatusId",
                table: "Plates");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Plates");

            migrationBuilder.AddColumn<bool>(
                name: "IsReserved",
                table: "Plates",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
