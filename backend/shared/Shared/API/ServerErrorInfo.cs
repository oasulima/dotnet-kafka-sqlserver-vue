namespace Shared.API
{
    public readonly record struct ServerErrorInfo
    {
        public string Message { get; init; }
        public string Detail { get; init; }
    }
}
