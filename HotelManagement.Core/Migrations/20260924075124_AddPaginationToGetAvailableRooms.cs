using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddPaginationToGetAvailableRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP FUNCTION IF EXISTS get_available_rooms(DATE, DATE);
DROP FUNCTION IF EXISTS get_available_rooms(DATE, DATE, INTEGER, INTEGER);

CREATE OR REPLACE FUNCTION get_available_rooms(
    p_start_date DATE,
    p_end_date   DATE,
    p_page       INTEGER DEFAULT 1,
    p_page_size  INTEGER DEFAULT 10
)
RETURNS TABLE (
    id             INTEGER,
    room_number    TEXT,
    room_type      TEXT,
    number_of_beds INTEGER,
    status         TEXT,
    created_at     TIMESTAMP WITHOUT TIME ZONE,
    updated_at     TIMESTAMP WITHOUT TIME ZONE
)
LANGUAGE plpgsql
STABLE
AS $$
BEGIN
    IF p_start_date IS NULL OR p_end_date IS NULL THEN
        RAISE EXCEPTION 'Both start_date and end_date are required.' USING ERRCODE = '22004';
    END IF;

    IF p_start_date >= p_end_date THEN
        RAISE EXCEPTION 'start_date (%) must be before end_date (%).', p_start_date, p_end_date USING ERRCODE = '22023';
    END IF;

    RETURN QUERY
    SELECT
        r.id,
        r.room_number,
        r.room_type,
        r.number_of_beds,
        r.status,
        r.created_at,
        r.updated_at
    FROM rooms r
    WHERE
        r.is_deleted = FALSE
        AND r.status IN ('Free', 'Booked')
        AND NOT EXISTS (
            SELECT 1
            FROM bookings b
            WHERE b.room_id    = r.id
              AND b.is_deleted = FALSE
              AND b.check_in   < p_end_date::TIMESTAMP WITHOUT TIME ZONE
              AND b.check_out  > p_start_date::TIMESTAMP WITHOUT TIME ZONE
        )
    ORDER BY r.id
    LIMIT GREATEST(COALESCE(p_page_size, 10), 1)
    OFFSET (GREATEST(COALESCE(p_page, 1), 1) - 1) * GREATEST(COALESCE(p_page_size, 10), 1);
END;
$$;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP FUNCTION IF EXISTS get_available_rooms(DATE, DATE, INTEGER, INTEGER);

CREATE OR REPLACE FUNCTION get_available_rooms(
    p_start_date DATE,
    p_end_date   DATE
)
RETURNS TABLE (
    ""Id""           INTEGER,
    ""RoomNumber""   TEXT,
    ""RoomType""     TEXT,
    ""NumberOfBeds"" INTEGER,
    ""Status""       TEXT,
    ""CreatedAt""    TIMESTAMP WITHOUT TIME ZONE,
    ""UpdatedAt""    TIMESTAMP WITHOUT TIME ZONE
)
LANGUAGE plpgsql
STABLE
AS $$
BEGIN
    IF p_start_date IS NULL OR p_end_date IS NULL THEN
        RAISE EXCEPTION 'Both start_date and end_date are required.' USING ERRCODE = '22004';
    END IF;

    IF p_start_date >= p_end_date THEN
        RAISE EXCEPTION 'start_date (%) must be before end_date (%).', p_start_date, p_end_date USING ERRCODE = '22023';
    END IF;

    RETURN QUERY
    SELECT
        r.id,
        r.room_number,
        r.room_type,
        r.number_of_beds,
        r.status,
        r.created_at,
        r.updated_at
    FROM rooms r
    WHERE
        r.is_deleted = FALSE
        AND r.status IN ('Free', 'Booked')
        AND NOT EXISTS (
            SELECT 1
            FROM bookings b
            WHERE b.room_id    = r.id
              AND b.is_deleted = FALSE
              AND b.check_in   < p_end_date::TIMESTAMP WITHOUT TIME ZONE
              AND b.check_out  > p_start_date::TIMESTAMP WITHOUT TIME ZONE
        )
    ORDER BY r.id;
END;
$$;
");
        }
    }
}
