namespace NavArgs.Generator;

internal interface INavDestination
{
    public string Route { get; }
    public INavArgs GetArgs();
}