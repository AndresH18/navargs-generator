namespace NavArgs.Generator;

internal interface INavArgs
{
    IDictionary<string, object?> ToDictionary();
}