using System.ComponentModel.DataAnnotations;
using NavArgs.Abstractions.Sample.Space;

namespace NavArgs.Abstractions.Sample;

[NavDestination(Route = "Details", ArgsName = "DetailArgs", Mode = GenerationMode.Strict)]
public partial class DetailsDestination : INavDestination
{
    #pragma warning disable CS8618
    public Guid A { get; set; }

    public int Id { get; set; }
    public int? Id2 { get; set; }
    public Nullable<int> Id2A { get; set; }
    public string Id3 { get; set; }
    public string? Id4 { get; set; }
    public SomeClass SomeClass1 { get; set; }
    public SomeClass? SomeClass2 { get; set; }
    public SpaceClass SpaceClass { get; set; }
    public SpaceClass? SpaceClass2 { get; set; }
    [IgnoreNavProperty] public object Complex { get; set; } = null!;
    #pragma warning restore CS8618

    void M()
    {
    }
}