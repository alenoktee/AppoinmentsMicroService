namespace Appointments.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {

    }

    public NotFoundException(string name, object key) : base($"Сущность \"{name}\" ({key}) не найдена.")
    {

    }
}
