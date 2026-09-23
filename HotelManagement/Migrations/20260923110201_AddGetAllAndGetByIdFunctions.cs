using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddGetAllAndGetByIdFunctions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
-- Users
CREATE OR REPLACE FUNCTION get_all_users()
RETURNS SETOF users
LANGUAGE sql
STABLE
AS $$
    SELECT * FROM users WHERE is_deleted = FALSE ORDER BY id;
$$;

CREATE OR REPLACE FUNCTION get_user_by_id(p_id INTEGER)
RETURNS SETOF users
LANGUAGE sql
STABLE
AS $$
    SELECT * FROM users WHERE id = p_id AND is_deleted = FALSE;
$$;

-- Rooms
CREATE OR REPLACE FUNCTION get_all_rooms()
RETURNS SETOF rooms
LANGUAGE sql
STABLE
AS $$
    SELECT * FROM rooms WHERE is_deleted = FALSE ORDER BY id;
$$;

CREATE OR REPLACE FUNCTION get_room_by_id(p_id INTEGER)
RETURNS SETOF rooms
LANGUAGE sql
STABLE
AS $$
    SELECT * FROM rooms WHERE id = p_id AND is_deleted = FALSE;
$$;

-- Bookings
CREATE OR REPLACE FUNCTION get_all_bookings()
RETURNS SETOF bookings
LANGUAGE sql
STABLE
AS $$
    SELECT * FROM bookings WHERE is_deleted = FALSE ORDER BY id;
$$;

CREATE OR REPLACE FUNCTION get_booking_by_id(p_id INTEGER)
RETURNS SETOF bookings
LANGUAGE sql
STABLE
AS $$
    SELECT * FROM bookings WHERE id = p_id AND is_deleted = FALSE;
$$;

-- Currencies
CREATE OR REPLACE FUNCTION get_all_currencies()
RETURNS SETOF currencies
LANGUAGE sql
STABLE
AS $$
    SELECT * FROM currencies WHERE is_deleted = FALSE ORDER BY id;
$$;

CREATE OR REPLACE FUNCTION get_currency_by_id(p_id INTEGER)
RETURNS SETOF currencies
LANGUAGE sql
STABLE
AS $$
    SELECT * FROM currencies WHERE id = p_id AND is_deleted = FALSE;
$$;

-- Prices
CREATE OR REPLACE FUNCTION get_all_prices()
RETURNS SETOF prices
LANGUAGE sql
STABLE
AS $$
    SELECT * FROM prices WHERE is_deleted = FALSE ORDER BY id;
$$;

CREATE OR REPLACE FUNCTION get_price_by_id(p_id INTEGER)
RETURNS SETOF prices
LANGUAGE sql
STABLE
AS $$
    SELECT * FROM prices WHERE id = p_id AND is_deleted = FALSE;
$$;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP FUNCTION IF EXISTS get_all_users();
DROP FUNCTION IF EXISTS get_user_by_id(INTEGER);
DROP FUNCTION IF EXISTS get_all_rooms();
DROP FUNCTION IF EXISTS get_room_by_id(INTEGER);
DROP FUNCTION IF EXISTS get_all_bookings();
DROP FUNCTION IF EXISTS get_booking_by_id(INTEGER);
DROP FUNCTION IF EXISTS get_all_currencies();
DROP FUNCTION IF EXISTS get_currency_by_id(INTEGER);
DROP FUNCTION IF EXISTS get_all_prices();
DROP FUNCTION IF EXISTS get_price_by_id(INTEGER);
");
        }
    }
}
