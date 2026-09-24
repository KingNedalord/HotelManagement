using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagement.Migrations
{
    /// <inheritdoc />
    public partial class ApplySnakeCaseAndBaseModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookings_rooms_room_id",
                table: "bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_bookings_users_user_id",
                table: "bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_prices_currencies_currency_id",
                table: "prices");

            migrationBuilder.DropForeignKey(
                name: "FK_prices_rooms_room_id",
                table: "prices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_rooms",
                table: "rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_prices",
                table: "prices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_currencies",
                table: "currencies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_bookings",
                table: "bookings");

            migrationBuilder.RenameIndex(
                name: "IX_prices_room_id",
                table: "prices",
                newName: "ix_prices_room_id");

            migrationBuilder.RenameIndex(
                name: "IX_prices_currency_id",
                table: "prices",
                newName: "ix_prices_currency_id");

            migrationBuilder.RenameIndex(
                name: "IX_bookings_user_id",
                table: "bookings",
                newName: "ix_bookings_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_bookings_room_id",
                table: "bookings",
                newName: "ix_bookings_room_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_rooms",
                table: "rooms",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_prices",
                table: "prices",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_currencies",
                table: "currencies",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bookings",
                table: "bookings",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_bookings_rooms_room_id",
                table: "bookings",
                column: "room_id",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_bookings_users_user_id",
                table: "bookings",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_prices_currencies_currency_id",
                table: "prices",
                column: "currency_id",
                principalTable: "currencies",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_prices_rooms_room_id",
                table: "prices",
                column: "room_id",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bookings_rooms_room_id",
                table: "bookings");

            migrationBuilder.DropForeignKey(
                name: "fk_bookings_users_user_id",
                table: "bookings");

            migrationBuilder.DropForeignKey(
                name: "fk_prices_currencies_currency_id",
                table: "prices");

            migrationBuilder.DropForeignKey(
                name: "fk_prices_rooms_room_id",
                table: "prices");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_rooms",
                table: "rooms");

            migrationBuilder.DropPrimaryKey(
                name: "pk_prices",
                table: "prices");

            migrationBuilder.DropPrimaryKey(
                name: "pk_currencies",
                table: "currencies");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bookings",
                table: "bookings");

            migrationBuilder.RenameIndex(
                name: "ix_prices_room_id",
                table: "prices",
                newName: "IX_prices_room_id");

            migrationBuilder.RenameIndex(
                name: "ix_prices_currency_id",
                table: "prices",
                newName: "IX_prices_currency_id");

            migrationBuilder.RenameIndex(
                name: "ix_bookings_user_id",
                table: "bookings",
                newName: "IX_bookings_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_bookings_room_id",
                table: "bookings",
                newName: "IX_bookings_room_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_rooms",
                table: "rooms",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_prices",
                table: "prices",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_currencies",
                table: "currencies",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_bookings",
                table: "bookings",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_bookings_rooms_room_id",
                table: "bookings",
                column: "room_id",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_bookings_users_user_id",
                table: "bookings",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_prices_currencies_currency_id",
                table: "prices",
                column: "currency_id",
                principalTable: "currencies",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_prices_rooms_room_id",
                table: "prices",
                column: "room_id",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
