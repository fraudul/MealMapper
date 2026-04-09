using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class AddRecommendationValueObjects : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "FoodPlaces",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Location_Latitude = table.Column<double>(type: "double precision", nullable: false),
                Location_Longitude = table.Column<double>(type: "double precision", nullable: false),
                Location_City = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                average_price_per_person = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                rating = table.Column<double>(type: "double precision", precision: 3, scale: 2, nullable: false),
                address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                is_open_now = table.Column<bool>(type: "boolean", nullable: false),
                external_map_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_food_places", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "Recipes",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                estimated_cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                cooking_time_minutes = table.Column<int>(type: "integer", nullable: false),
                ingredients = table.Column<string>(type: "text", nullable: false),
                steps = table.Column<string>(type: "text", nullable: false),
                cuisine = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_recipes", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "RecommendationSessions",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_location_latitude = table.Column<double>(type: "double precision", nullable: false),
                user_location_longitude = table.Column<double>(type: "double precision", nullable: false),
                user_location_city = table.Column<string>(type: "text", nullable: false),
                Budget_Min = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                Budget_Max = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                Preferences_ActionType = table.Column<int>(type: "integer", nullable: false),
                Preferences_IsFastFoodAllowed = table.Column<bool>(type: "boolean", nullable: false),
                Preferences_DistancePref = table.Column<int>(type: "integer", nullable: false),
                Preferences_CuisinePreference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                Preferences_OnlyNearby = table.Column<bool>(type: "boolean", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_recommendation_sessions", x => x.id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "FoodPlaces",
            schema: "public");

        migrationBuilder.DropTable(
            name: "Recipes",
            schema: "public");

        migrationBuilder.DropTable(
            name: "RecommendationSessions",
            schema: "public");
    }
}
