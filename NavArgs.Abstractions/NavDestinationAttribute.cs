namespace NavArgs.Abstractions;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
internal class NavDestinationAttribute : Attribute
{
    public string Route { get; init; } = string.Empty;
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
internal class IgnoreNavPropertyAttribute : Attribute;
