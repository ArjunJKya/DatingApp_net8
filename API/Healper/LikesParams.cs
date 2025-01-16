using System;

namespace API.Healper;

public class LikesParams:PaginationParams
{
    public int userid { get; set; }
    public required string Predicate { get; set; } = "liked";

}
