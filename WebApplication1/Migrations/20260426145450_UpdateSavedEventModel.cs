using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSavedEventModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SavedEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SavedAt",
                table: "SavedEvents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_SavedEvents_EventId",
                table: "SavedEvents",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedEvents_Events_EventId",
                table: "SavedEvents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedEvents_Users_UserId",
                table: "SavedEvents",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedEvents_Events_EventId",
                table: "SavedEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedEvents_Users_UserId",
                table: "SavedEvents");

            migrationBuilder.DropIndex(
                name: "IX_SavedEvents_EventId",
                table: "SavedEvents");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SavedEvents");

            migrationBuilder.DropColumn(
                name: "SavedAt",
                table: "SavedEvents");
        }
    }
}
