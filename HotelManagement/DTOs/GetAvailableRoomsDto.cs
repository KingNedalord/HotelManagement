namespace HotelManagement.DTOs;

public class GetAvailableRoomsDto : PaginationRequestDto
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}