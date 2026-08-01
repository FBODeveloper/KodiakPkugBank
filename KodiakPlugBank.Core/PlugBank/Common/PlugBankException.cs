namespace KodiakPlugBank.Core.PlugBank.Common;

public class PlugBankException : Exception
{
    public int StatusCode { get; }
    public PlugBankError? Error { get; }

    public PlugBankException(int statusCode, string message, PlugBankError? error = null)
        : base(message)
    {
        StatusCode = statusCode;
        Error = error;
    }
}
