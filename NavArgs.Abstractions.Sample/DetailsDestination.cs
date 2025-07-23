
using System.ComponentModel.DataAnnotations;

namespace NavArgs.Abstractions.Sample;

[NavDestination(Route = "Details", ArgsName = "DetailArgs")]
public partial class DetailsDestination : INavDestination
{
    public int Id { get; set; }
    public int? Id2 { get; set; }
    
    // [IgnoreNavProperty]
    public B? Complex { get; set; } = null!;
}

public class B
{
    
}