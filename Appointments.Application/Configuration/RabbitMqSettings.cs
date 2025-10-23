namespace Appointments.Application.Configuration;

public class RabbitMqSettings
{
    public const string SectionName = "RabbitMq";
    public string Host { get; set; } = string.Empty;
    public string VirtualHost { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
