namespace CarRental.Application.Common;

public record CarWithServiceDto(
    Guid CarId,
    string Type,
    string Make,
    string Model,
    int Year,
    string Location,
    ServiceDto Service);

public record ServiceDto(Guid ServiceId, DateTime Date);