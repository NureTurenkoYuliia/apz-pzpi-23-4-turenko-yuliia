namespace CleanArium.Contracts.Aquariums;

public class UpdateAquariumRequest
{
    public long AquariumId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
}
