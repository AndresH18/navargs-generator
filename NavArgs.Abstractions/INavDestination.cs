namespace NavArgs.Abstractions;

internal interface INavDestination
{
    public string Route { get; }
    public INavArgs GetArgs();
}