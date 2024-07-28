using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AndOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserPreference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPreferences_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DefaultProgramForExtension",
                columns: table => new
                {
                    UserPreferenceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Extension = table.Column<string>(type: "text", nullable: false),
                    Program = table.Column<string>(type: "text", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultProgramForExtension", x => new { x.Extension, x.Program, x.UserPreferenceId });
                    table.ForeignKey(
                        name: "FK_DefaultProgramForExtension_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DefaultProgramForExtension_UserPreferences_UserPreferenceId",
                        column: x => x.UserPreferenceId,
                        principalTable: "UserPreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultProgramForExtension_ApplicationUserId",
                table: "DefaultProgramForExtension",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultProgramForExtension_UserPreferenceId",
                table: "DefaultProgramForExtension",
                column: "UserPreferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_UserId",
                table: "UserPreferences",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DefaultProgramForExtension");

            migrationBuilder.DropTable(
                name: "UserPreferences");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "AspNetUsers");
        }
    }
}
