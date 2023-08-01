using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeShopMVC.Migrations
{
    /// <inheritdoc />
    public partial class OrderIdError : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_items_orders_order_id",
                table: "items");

            migrationBuilder.AlterColumn<int>(
                name: "order_id",
                table: "items",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "fk_items_orders_order_id",
                table: "items",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_items_orders_order_id",
                table: "items");

            migrationBuilder.AlterColumn<int>(
                name: "order_id",
                table: "items",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_items_orders_order_id",
                table: "items",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
