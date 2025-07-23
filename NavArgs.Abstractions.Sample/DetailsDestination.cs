
using System.ComponentModel.DataAnnotations;

namespace NavArgs.Abstractions.Sample;

[NavDestination(Route = "Details", ArgsName = "DetailArgs")]
public partial class DetailsDestination : INavDestination
{
    public int Id { get; set; }
    [IgnoreNavProperty]
    public object Complex { get; set; } = null!;
}