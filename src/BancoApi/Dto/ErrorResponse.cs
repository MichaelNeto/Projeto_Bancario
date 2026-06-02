using System.Collections.Generic;

namespace BancoApi.Dto;

public class ErrorResponse
{
    public int Status { get; set; }
    public string Message { get; set; } = null!;
    public object? Errors { get; set; }
}
