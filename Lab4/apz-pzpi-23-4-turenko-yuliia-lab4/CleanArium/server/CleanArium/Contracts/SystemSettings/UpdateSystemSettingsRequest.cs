namespace CleanArium.Contracts.SystemSettings;

public class UpdateSystemSettingsRequest
{
    public int MaxAquariumsPerUser { get; set; }
    public int MaxDevicesPerAquarium { get; set; }
    public int MaxAlarmRulesPerDevice { get; set; }
    public int MaxScheduledCommandsPerDevice { get; set; }
}
