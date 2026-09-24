using HotelManagement.Core.Models.Common;

namespace HotelManagement.Core.Models.Currencies;

public class Currency : BaseModel
{
    public string Code { get; set; }

    public string Name { get; set; }
}