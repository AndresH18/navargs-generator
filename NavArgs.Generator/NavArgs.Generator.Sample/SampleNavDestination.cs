using System;
using System.Collections.Generic;
using Generators;

namespace NavArgs.Generator.Sample;

[NavDestination]
public partial class SampleNavDestination : INavDestination
{
    public string Route { get; set; }
    public string? Name { get; set; }
    public int Id { get; set; }
    public Uri Uri { get; set; }
}

public class SomeArgs : INavArgs
{
    public string Name { get; set; }
    public int Id { get; set; }

    public IDictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>()
        {
            [nameof(Name)] = Name,
            [nameof(Id)] = Id
        };
    }
}