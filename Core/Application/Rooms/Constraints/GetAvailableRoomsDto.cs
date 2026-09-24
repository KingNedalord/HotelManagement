using HotelManagement.Core.Application.Common.Constraints;

namespace HotelManagement.Core.Application.Rooms.Constraints;

public class GetAvailableRoomsDto : PaginationRequestDto
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}