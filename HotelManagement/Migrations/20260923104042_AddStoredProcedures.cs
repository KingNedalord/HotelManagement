using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddStoredProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop any existing functions/procedures if present
            migrationBuilder.Sql(@"
DROP FUNCTION IF EXISTS create_booking CASCADE;
DROP PROCEDURE IF EXISTS create_booking CASCADE;
DROP FUNCTION IF EXISTS get_available_rooms CASCADE;
DROP PROCEDURE IF EXISTS get_available_rooms CASCADE;
");

            // Stored procedure: create_booking
            migrationBuilder.Sql(@"
CREATE OR REPLACE PROCEDURE create_booking(
    IN p_room_id INTEGER,
    IN p_user_id INTEGER,
    IN p_check_in TIMESTAMP WITHOUT TIME ZONE,
    IN p_check_out TIMESTAMP WITHOUT TIME ZONE,
    IN p_is_paid BOOLEAN,
    OUT p_id INTEGER,
    OUT p_created_at TIMESTAMP WITHOUT TIME ZONE,
    OUT p_updated_at TIMESTAMP WITHOUT TIME ZONE
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_now TIMESTAMP WITHOUT TIME ZONE := LOCALTIMESTAMP;
BEGIN
    -- 1. Validate dates are provided
    IF p_check_in IS NULL OR p_check_out IS NULL THEN
        RAISE EXCEPTION 'Both check_in and check_out dates are required.' USING ERRCODE = '22004';
    END IF;

    -- 2. Validate checkout is after checkin
    IF p_check_out <= p_check_in THEN
        RAISE EXCEPTION 'Check-out date must be after check-in date.' USING ERRCODE = '22023';
    END IF;

    -- 3. Check room exists and is not soft-deleted
    IF NOT EXISTS (
        SELECT 1 FROM rooms WHERE id = p_room_id AND is_deleted = FALSE
    ) THEN
        RAISE EXCEPTION 'Room with id % was not found.', p_room_id USING ERRCODE = 'P0002';
    END IF;

    -- 4. Check user exists and is not soft-deleted
    IF NOT EXISTS (
        SELECT 1 FROM users WHERE id = p_user_id AND is_deleted = FALSE
    ) THEN
        RAISE EXCEPTION 'User with id % was not found.', p_user_id USING ERRCODE = 'P0002';
    END IF;

    -- 5. Check room is available throughout the requested time period
    IF EXISTS (
        SELECT 1
        FROM bookings b
        WHERE b.room_id = p_room_id
          AND b.is_deleted = FALSE
          AND b.check_in < p_check_out
          AND b.check_out > p_check_in
    ) THEN
        RAISE EXCEPTION 'Room % is not available for the requested time period.', p_room_id USING ERRCODE = '23P01';
    END IF;

    -- 6. Insert new booking and assign output parameters
    INSERT INTO bookings (room_id, user_id, check_in, check_out, is_paid, created_at, updated_at, is_deleted)
    VALUES (p_room_id, p_user_id, p_check_in, p_check_out, p_is_paid, v_now, v_now, FALSE)
    RETURNING id, created_at, updated_at INTO p_id, p_created_at, p_updated_at;
END;
$$;
");

            // Function: get_available_rooms
            migrationBuilder.Sql(@"
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP PROCEDURE IF EXISTS create_booking;
DROP FUNCTION IF EXISTS get_available_rooms;
");
        }
    }
}
