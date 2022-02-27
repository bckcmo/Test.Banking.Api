namespace Test.Banking.Api.Types;

public class ServiceResult<T>
{
    public T? Result { get; set; }

    public List<string> Errors { get; set; } = new();
}