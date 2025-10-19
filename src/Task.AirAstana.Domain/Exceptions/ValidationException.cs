namespace Task.AirAstana.Domain.Exceptions;

public class ValidationException : Exception
{
    //TODO: Дополню если будет время 
    
    public IDictionary<string, string[]> Errors { get; }
    
    public ValidationException(IDictionary<string, string[]> errors) : base("Произошла одна или несколько ошибок при проверке данных.")
    {
        Errors = errors;
    }

}