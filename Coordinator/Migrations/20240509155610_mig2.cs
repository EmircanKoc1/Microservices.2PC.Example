using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Coordinator.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Nodes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("c6a76381-f1db-435c-9e7b-a20d81d701af"), "Order.API" },
                    { new Guid("d6435bfe-194a-42b8-b602-caa8be251eba"), "Order.API" },
                    { new Guid("f4c52f26-1c84-4533-8de0-7ef1617963d0"), "Order.API" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("c6a76381-f1db-435c-9e7b-a20d81d701af"));

            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("d6435bfe-194a-42b8-b602-caa8be251eba"));

            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("f4c52f26-1c84-4533-8de0-7ef1617963d0"));
        }
    }
}
