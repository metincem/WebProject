using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KelimeOyunu.DataAccess.Migrations
{
    public partial class InitKelime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kelimeler",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KelimeUzunluk = table.Column<int>(type: "int", nullable: false),
                    SoruKelime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SonKullanma = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kelimeler", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Oturumlar",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdSoyad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OturumSaati = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Puan = table.Column<int>(type: "int", nullable: false),
                    Sonlandi = table.Column<bool>(type: "bit", nullable: false),
                    ArtanSure = table.Column<int>(type: "int", nullable: false),
                    Soru1 = table.Column<int>(type: "int", nullable: false),
                    Soru2 = table.Column<int>(type: "int", nullable: false),
                    Soru3 = table.Column<int>(type: "int", nullable: false),
                    Soru4 = table.Column<int>(type: "int", nullable: false),
                    Soru5 = table.Column<int>(type: "int", nullable: false),
                    Soru6 = table.Column<int>(type: "int", nullable: false),
                    Soru7 = table.Column<int>(type: "int", nullable: false),
                    Soru8 = table.Column<int>(type: "int", nullable: false),
                    Soru9 = table.Column<int>(type: "int", nullable: false),
                    Soru10 = table.Column<int>(type: "int", nullable: false),
                    Soru11 = table.Column<int>(type: "int", nullable: false),
                    Soru12 = table.Column<int>(type: "int", nullable: false),
                    Soru13 = table.Column<int>(type: "int", nullable: false),
                    Soru14 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oturumlar", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Surecler",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SrConnectionID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OturumID = table.Column<int>(type: "int", nullable: false),
                    SureDurduruldu = table.Column<bool>(type: "bit", nullable: false),
                    BaslanacakSure = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KalanSure = table.Column<int>(type: "int", nullable: false),
                    Soru1AHa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Soru1Durum = table.Column<int>(type: "int", nullable: false),
                    Soru2AHa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Soru2Durum = table.Column<int>(type: "int", nullable: false),
                    Soru3AHa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Soru3Durum = table.Column<int>(type: "int", nullable: false),
                    Soru4AHa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Soru4Durum = table.Column<int>(type: "int", nullable: false),
                    Soru5AHa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Soru5Durum = table.Column<int>(type: "int", nullable: false),
                    Soru6AHa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Soru6Durum = table.Column<int>(type: "int", nullable: false),
                    Soru7AHa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Soru7Durum = table.Column<int>(type: "int", nullable: false),
                    Soru8AHa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Soru8Durum = table.Column<int>(type: "int", nullable: false),
                    Soru9AHa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Soru9Durum = table.Column<int>(type: "int", nullable: false),
                    Soru10AHa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Soru10Durum = table.Column<int>(type: "int", nullable: false),
                    Soru11AHa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Soru11Durum = table.Column<int>(type: "int", nullable: false),
                    Soru12AHa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Soru12Durum = table.Column<int>(type: "int", nullable: false),
                    Soru13AHa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Soru13Durum = table.Column<int>(type: "int", nullable: false),
                    Soru14AHa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Soru14Durum = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surecler", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Kelimeler");

            migrationBuilder.DropTable(
                name: "Oturumlar");

            migrationBuilder.DropTable(
                name: "Surecler");
        }
    }
}
