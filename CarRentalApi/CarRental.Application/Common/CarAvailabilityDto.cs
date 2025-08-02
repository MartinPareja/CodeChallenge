public record CarAvailabilityDto(
    Guid CarId,
    string Make,
    string Model,
    int Year,
    string Location,
    bool IsAvailable);