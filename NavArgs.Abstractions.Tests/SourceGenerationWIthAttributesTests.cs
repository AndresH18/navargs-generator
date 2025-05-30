using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NavArgs.Generator;

namespace NavArgs.Abstractions.Tests;

public partial class SourceGenerationWIthAttributesTests
{
    [Fact]
    public void GenerateWithoutRouteParameter()
    {
        // create an instance of the source generator
        var generator = new NavDestinationGenerator();

        var driver = CSharpGeneratorDriver.Create(generator);
        var compilation = CSharpCompilation.Create(nameof(SourceGenerationWIthAttributesTests),
            new[]
            {
                CSharpSyntaxTree.ParseText(Constants.AccountDetailsWithoutRouteClass)
            },
            new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
            });

        var runResult = driver.RunGenerators(compilation).GetRunResult();

        var generatedFileSyntax = runResult.GeneratedTrees.Single(t => t.FilePath.EndsWith("AccountDetails.g.cs"));

        var generatedFileText = generatedFileSyntax.GetText().ToString();
        generatedFileText = SpaceRegex().Replace(generatedFileText, " ");
        
        Assert.Equal(SpaceRegex().Replace(Constants.ExpectedAccountDetailsWithoutRouteClass, " "), generatedFileText,
            ignoreLineEndingDifferences: true, ignoreWhiteSpaceDifferences: true);
    }

    [GeneratedRegex(@"(\r\n|\r|\n)*")]
    private static partial Regex SpaceRegex();
}