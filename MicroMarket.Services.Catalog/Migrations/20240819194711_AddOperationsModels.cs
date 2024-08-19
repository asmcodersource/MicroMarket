using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroMarket.Services.Catalog.Migrations
{
    /// <inheritdoc />
    public partial class AddOperationsModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemsClaimOperations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsClaimOperations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxOperations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    AggregationId = table.Column<Guid>(type: "uuid", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    OperationType = table.Column<int>(type: "integer", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxOperations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemToClaim",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductQuantity = table.Column<int>(type: "integer", nullable: false),
                    ItemsClaimOperationId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemToClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemToClaim_ItemsClaimOperations_ItemsClaimOperationId",
                        column: x => x.ItemsClaimOperationId,
                        principalTable: "ItemsClaimOperations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemToClaim_ItemsClaimOperationId",
                table: "ItemToClaim",
                column: "ItemsClaimOperationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemToClaim");

            migrationBuilder.DropTable(
                name: "OutboxOperations");

            migrationBuilder.DropTable(
                name: "ItemsClaimOperations");
        }
    }
}
