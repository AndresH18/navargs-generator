namespace NavArgs.Abstractions;

internal interface INavArgs
{
    IDictionary<string, object?> ToDictionary();
}