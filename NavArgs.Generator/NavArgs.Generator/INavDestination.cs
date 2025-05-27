namespace NavArgs.Generator;

public interface INavDestination
{
    public string Route { get; }
    public INavArgs GetArgs();
}