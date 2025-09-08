using System.ComponentModel.DataAnnotations;
using NavArgs.Abstractions.Sample.Space;

namespace NavArgs.Abstractions.Sample;

[NavDestination(Route = "Details", ArgsName = "DetailArgs", Mode = GenerationMode.Strict)]
public partial class DetailsDestination2 : INavDestination
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
        var dict = new Dictionary<string, object?>();
        var v1 = default(string);
        var v2 = "";
        if (dict.TryGetValue("", out var o1) && o1 is string s1)
        {
            v1 = s1;
        };

        new Cl(v1, v2);
        if (new object() is int i) ;
    }

    record Cl(string? V1, string V2);
}