using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class AddDriversAndCarss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicensePlate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    BodyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Experience = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarDriver",
                columns: table => new
                {
                    CarId = table.Column<int>(type: "int", nullable: false),
                    DriverId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarDriver", x => new { x.CarId, x.DriverId });
                    table.ForeignKey(
                        name: "FK_CarDriver_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarDriver_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "Id", "BodyType", "Brand", "Color", "IsAvailable", "LicensePlate", "Model", "Year" },
                values: new object[,]
                {
                    { 1, "Sedan", "Toyota", "Сірий", true, "AA1234BB", "Camry", 2020 },
                    { 2, "Sedan", "Skoda", "Білий", true, "BB5678CC", "Octavia", 2021 },
                    { 3, "Sedan", "Mercedes-Benz", "Чорний", true, "CC9012DD", "E-Class", 2022 },
                    { 4, "Minivan", "Volkswagen", "Синій", true, "DD3456EE", "Multivan", 2019 }
                });

            migrationBuilder.InsertData(
                table: "Drivers",
                columns: new[] { "Id", "Experience", "LicenseNumber", "Name", "Phone", "Rating" },
                values: new object[,]
                {
                    { 1, 5, "ВК123456", "Іван Петренко", "+380671234567", 4.8m },
                    { 2, 3, "АС789012", "Олена Коваленко", "+380502345678", 4.9m },
                    { 3, 7, "НМ345678", "Андрій Шевченко", "+380933456789", 4.7m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarDriver_DriverId",
                table: "CarDriver",
                column: "DriverId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarDriver");

            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "Drivers");
        }
    }
}
