namespace CodingChallenge.WebApi.Models;

public record struct AppInformationResponse(string Description, string? Build, string MachineName);