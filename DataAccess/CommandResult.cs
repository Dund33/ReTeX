namespace DataAccess
{
    public record CommandResult(bool IsSuccess, byte[]? Result);
}
