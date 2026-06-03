namespace Application.DTOs.Users;

public class UserActivityDailyDto
{
    public DateOnly Date { get; set; }
    public int ActionsCount { get; set; }
}
