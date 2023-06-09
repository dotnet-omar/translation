﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Translation.Migrations
{
    public partial class initializeTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "translation");

            migrationBuilder.CreateTable(
                name: "languages",
                schema: "translation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    is_rtl = table.Column<bool>(type: "bit", nullable: false),
                    is_locked = table.Column<bool>(type: "bit", nullable: false),
                    order = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_languages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "translation_files",
                schema: "translation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    file_path = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    file_hash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_translation_files", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "translation_keys",
                schema: "translation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    module = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_translation_keys", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "translation_values",
                schema: "translation",
                columns: table => new
                {
                    language_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    translation_key_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    default_value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    overridden_value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_translation_values", x => new { x.translation_key_id, x.language_id });
                    table.ForeignKey(
                        name: "FK_translation_values_languages_language_id",
                        column: x => x.language_id,
                        principalSchema: "translation",
                        principalTable: "languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_translation_values_translation_keys_translation_key_id",
                        column: x => x.translation_key_id,
                        principalSchema: "translation",
                        principalTable: "translation_keys",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_languages_code",
                schema: "translation",
                table: "languages",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_translation_files_file_path",
                schema: "translation",
                table: "translation_files",
                column: "file_path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_translation_values_language_id",
                schema: "translation",
                table: "translation_values",
                column: "language_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "translation_files",
                schema: "translation");

            migrationBuilder.DropTable(
                name: "translation_values",
                schema: "translation");

            migrationBuilder.DropTable(
                name: "languages",
                schema: "translation");

            migrationBuilder.DropTable(
                name: "translation_keys",
                schema: "translation");
        }
    }
}
