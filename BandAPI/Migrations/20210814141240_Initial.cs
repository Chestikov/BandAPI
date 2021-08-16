using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BandAPI.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bands",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Founded = table.Column<DateTime>(nullable: false),
                    MainGenre = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Albums",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 200, nullable: false),
                    Description = table.Column<string>(maxLength: 400, nullable: true),
                    BandId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Albums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Albums_Bands_BandId",
                        column: x => x.BandId,
                        principalTable: "Bands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Bands",
                columns: new[] { "Id", "Founded", "MainGenre", "Name" },
                values: new object[,]
                {
                    { new Guid("a7c5fded-f96c-4e5e-b6ed-dffd74ce0e7c"), new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Heavy Metal", "Metallica" },
                    { new Guid("2c2278a5-63ac-4be6-b3d7-2c52ca339e04"), new DateTime(1985, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rock", "Guns N Roses" },
                    { new Guid("c4160eb1-42af-4912-930f-0bc21fad3760"), new DateTime(1965, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Disco", "ABBA" },
                    { new Guid("0276a268-33e5-4858-8589-4fa10ffbd5f9"), new DateTime(1991, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alternative", "Oasis" },
                    { new Guid("46ad6b44-d02c-4db6-8104-d0c73520bac6"), new DateTime(1981, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pop", "A-ha" }
                });

            migrationBuilder.InsertData(
                table: "Albums",
                columns: new[] { "Id", "BandId", "Description", "Title" },
                values: new object[,]
                {
                    { new Guid("55c45c62-2be4-4764-bba3-c7d4b6fdc0fe"), new Guid("a7c5fded-f96c-4e5e-b6ed-dffd74ce0e7c"), "One pf the best heavy metal albuns ever", "Master Of Puppets" },
                    { new Guid("16945b9e-cfb2-455a-b5e2-be67f05bf33f"), new Guid("2c2278a5-63ac-4be6-b3d7-2c52ca339e04"), "Amazing Rock album with raw sound", "Appetite for Destruction" },
                    { new Guid("9f999965-b620-4ac0-bdf7-d0a37b6b8b5f"), new Guid("c4160eb1-42af-4912-930f-0bc21fad3760"), "Very groovy albim", "Waterloo" },
                    { new Guid("509ecd4d-d3f3-40a8-8973-c00b4153d417"), new Guid("0276a268-33e5-4858-8589-4fa10ffbd5f9"), "Arguably one og the best albums by Oasis", "Be Here Now" },
                    { new Guid("e78d2f07-22e8-4cee-bcd5-c952a069ca8f"), new Guid("46ad6b44-d02c-4db6-8104-d0c73520bac6"), "Awesome Debut album by A-Ha", "Hunting Hight and Low" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Albums_BandId",
                table: "Albums",
                column: "BandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Albums");

            migrationBuilder.DropTable(
                name: "Bands");
        }
    }
}
