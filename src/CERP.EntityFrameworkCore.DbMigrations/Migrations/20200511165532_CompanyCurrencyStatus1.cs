﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace CERP.Migrations
{
    public partial class CompanyCurrencyStatus1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyStatus",
                schema: "SETUP",
                table: "CompanyCurrencies");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "SETUP",
                table: "CompanyCurrencies",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                schema: "SETUP",
                table: "CompanyCurrencies");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyStatus",
                schema: "SETUP",
                table: "CompanyCurrencies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
