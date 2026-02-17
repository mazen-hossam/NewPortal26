using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBoys.API.Migrations
{
    /// <inheritdoc />
    public partial class addingdbconnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "prtl_Languages",
                schema: "dbo",
                columns: table => new
                {
                    Lang_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LCID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prtl_Languages", x => x.Lang_Id);
                });

            migrationBuilder.CreateTable(
                name: "prtl_news",
                schema: "dbo",
                columns: table => new
                {
                    News_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    News_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    News_img = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Owner_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    currentNews_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Published = table.Column<bool>(type: "bit", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prtl_news", x => x.News_Id);
                });

            migrationBuilder.CreateTable(
                name: "prtl_news_univ",
                schema: "dbo",
                columns: table => new
                {
                    News_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    News_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    News_img = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Owner_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    currentNews_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Published = table.Column<bool>(type: "bit", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prtl_news_univ", x => x.News_Id);
                });

            migrationBuilder.CreateTable(
                name: "prtl_News_Translations",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    News_Head = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    News_Abbr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    News_Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    News_Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lang_Id = table.Column<int>(type: "int", nullable: false),
                    Img_alt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    News_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prtl_News_Translations", x => x.id);
                    table.ForeignKey(
                        name: "FK_prtl_News_Translations_prtl_Languages_Lang_Id",
                        column: x => x.Lang_Id,
                        principalSchema: "dbo",
                        principalTable: "prtl_Languages",
                        principalColumn: "Lang_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_prtl_News_Translations_prtl_news_News_Id",
                        column: x => x.News_Id,
                        principalSchema: "dbo",
                        principalTable: "prtl_news",
                        principalColumn: "News_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prtl_news_univ_trans",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    News_Head = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    News_Abbr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    News_Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    News_Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lang_Id = table.Column<int>(type: "int", nullable: false),
                    Img_alt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    News_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prtl_news_univ_trans", x => x.id);
                    table.ForeignKey(
                        name: "FK_prtl_news_univ_trans_prtl_Languages_Lang_Id",
                        column: x => x.Lang_Id,
                        principalSchema: "dbo",
                        principalTable: "prtl_Languages",
                        principalColumn: "Lang_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_prtl_news_univ_trans_prtl_news_univ_News_Id",
                        column: x => x.News_Id,
                        principalSchema: "dbo",
                        principalTable: "prtl_news_univ",
                        principalColumn: "News_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_prtl_News_Translations_Lang_Id",
                schema: "dbo",
                table: "prtl_News_Translations",
                column: "Lang_Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_prtl_News_Translations_News_Id",
                schema: "dbo",
                table: "prtl_News_Translations",
                column: "News_Id");

            migrationBuilder.CreateIndex(
                name: "IX_prtl_news_univ_trans_Lang_Id",
                schema: "dbo",
                table: "prtl_news_univ_trans",
                column: "Lang_Id");

            migrationBuilder.CreateIndex(
                name: "IX_prtl_news_univ_trans_News_Id",
                schema: "dbo",
                table: "prtl_news_univ_trans",
                column: "News_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "prtl_News_Translations",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "prtl_news_univ_trans",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "prtl_news",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "prtl_Languages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "prtl_news_univ",
                schema: "dbo");
        }
    }
}
