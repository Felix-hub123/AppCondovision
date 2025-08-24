using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CondoVision.Data.Migrations
{
    /// <inheritdoc />
    public partial class FractionOwners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FractionOwners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FractionFloor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FractionBlock = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OwnerFullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OwnerEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WasDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FractionOwners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FractionOwners_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FractionOwners_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FractionOwners_UnitId",
                table: "FractionOwners",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_FractionOwners_UserId",
                table: "FractionOwners",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FractionOwners");
        }
    }
}
