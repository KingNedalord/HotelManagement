using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddPaginationToGetAllFunctions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP FUNCTION IF EXISTS get_all_users();
DROP FUNCTION IF EXISTS get_all_rooms();
DROP FUNCTION IF EXISTS get_all_bookings();
DROP FUNCTION IF EXISTS get_all_currencies();
DROP FUNCTION IF EXISTS get_all_prices();

-- Users
CREATE OR REPLACE FUNCTION get_all_users(p_page INTEGER DEFAULT 1, p_page_size INTEGER DEFAULT 10)
RETURNS SETOF users
LANGUAGE sql
STABLE
AS $$
    SELECT * FROM users
    WHERE is_deleted = FALSE
    ORDER BY id
    LIMIT GREATEST(COALESCE(p_page_size, 10), 1)
    OFFSET (GREATEST(COALESCE(p_page, 1), 1) - 1) * GREATEST(COALESCE(p_page_size, 10), 1);
$$;

-- Rooms
CREATE OR REPLACE FUNCTION get_all_rooms(p_page INTEGER DEFAULT 1, p_page_size INTEGER DEFAULT 10)
RETURNS SETOF rooms
LANGUAGE sql
STABLE
AS $$
    SELECT * FROM rooms
    WHERE is_deleted = FALSE
    ORDER BY id
    LIMIT GREATEST(COALESCE(p_page_size, 10), 1)
    OFFSET (GREATEST(COALESCE(p_page, 1), 1) - 1) * GREATEST(COALESCE(p_page_size, 10), 1);
$$;

-- Bookings
CREATE OR REPLACE FUNCTION get_all_bookings(p_page INTEGER DEFAULT 1, p_page_size INTEGER DEFAULT 10)
RETURNS SETOF bookings
LANGUAGE sql
STABLE
AS $$
    SELECT * FROM bookings
    WHERE is_deleted = FALSE
    ORDER BY id
    LIMIT GREATEST(COALESCE(p_page_size, 10), 1)
    OFFSET (GREATEST(COALESCE(p_page, 1), 1) - 1) * GREATEST(COALESCE(p_page_size, 10), 1);
$$;

-- Currencies
CREATE OR REPLACE FUNCTION get_all_currencies(p_page INTEGER DEFAULT 1, p_page_size INTEGER DEFAULT 10)
RETURNS SETOF currencies
LANGUAGE sql
STABLE
AS $$
    SELECT * FROM currencies
    WHERE is_deleted = FALSE
    ORDER BY id
    LIMIT GREATEST(COALESCE(p_page_size, 10), 1)
    OFFSET (GREATEST(COALESCE(p_page, 1), 1) - 1) * GREATEST(COALESCE(p_page_size, 10), 1);
$$;

-- Prices
CREATE OR REPLACE FUNCTION get_all_prices(p_page INTEGER DEFAULT 1, p_page_size INTEGER DEFAULT 10)
RETURNS SETOF prices
LANGUAGE sql
STABLE
AS $$
    SELECT * FROM prices
    WHERE is_deleted = FALSE
    ORDER BY id
    LIMIT GREATEST(COALESCE(p_page_size, 10), 1)
    OFFSET (GREATEST(COALESCE(p_page, 1), 1) - 1) * GREATEST(COALESCE(p_page_size, 10), 1);
$$;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP FUNCTION IF EXISTS get_all_users(INTEGER, INTEGER);
DROP FUNCTION IF EXISTS get_all_rooms(INTEGER, INTEGER);
DROP FUNCTION IF EXISTS get_all_bookings(INTEGER, INTEGER);
DROP FUNCTION IF EXISTS get_all_currencies(INTEGER, INTEGER);
DROP FUNCTION IF EXISTS get_all_prices(INTEGER, INTEGER);

CREATE OR REPLACE FUNCTION get_all_users()
RETURNS SETOF users
LANGUAGE sql
STABLE
AS $$
    SELECT * FROM users WHERE is_deleted = FALSE ORDER BY id;
$$;

CREATE OR REPLACE FUNCTION get_all_rooms()
RETURNS SETOF rooms
LANGUAGE sql
STABLE
AS $$
    SELECT * FROM rooms WHERE is_deleted = FALSE ORDER BY id;
$$;

CREATE OR REPLACE FUNCTION get_all_bookings()
RETURNS SETOF bookings
LANGUAGE sql
STABLE
AS $$
    SELECT * FROM bookings WHERE is_deleted = FALSE ORDER BY id;
$$;

CREATE OR REPLACE FUNCTION get_all_currencies()
RETURNS SETOF currencies
LANGUAGE sql
STABLE
AS $$
    SELECT * FROM currencies WHERE is_deleted = FALSE ORDER BY id;
$$;

CREATE OR REPLACE FUNCTION get_all_prices()
RETURNS SETOF prices
LANGUAGE sql
STABLE
AS $$
    SELECT * FROM prices WHERE is_deleted = FALSE ORDER BY id;
$$;
");
        }
    }
}
