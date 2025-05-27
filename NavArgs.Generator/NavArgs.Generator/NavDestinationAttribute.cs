namespace NavArgs.Generator;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
internal class NavDestinationAttribute : Attribute
{
    public string Route { get; init; } = string.Empty;
}