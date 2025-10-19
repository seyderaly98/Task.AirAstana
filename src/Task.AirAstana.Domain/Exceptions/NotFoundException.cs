namespace Task.AirAstana.Domain.Exceptions;

public class NotFoundException : Exception
{
    //TODO: Дополню 
    public NotFoundException(string name, object key) : base($"Сущность \"{name}\" ({key}) не найдена.")
    {
    }
}