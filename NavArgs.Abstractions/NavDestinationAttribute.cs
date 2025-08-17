namespace NavArgs.Abstractions;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
internal class NavDestinationAttribute : Attribute
{
    public string? Route { get; init; }
    public string? ArgsName { get; init; }
    public GenerationMode Mode { get; init; } = GenerationMode.Strict;
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
internal class IgnoreNavPropertyAttribute : Attribute;

internal enum GenerationMode
{
    Strict,
    Flexible
}