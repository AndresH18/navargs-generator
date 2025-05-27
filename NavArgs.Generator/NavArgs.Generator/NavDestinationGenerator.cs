using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NavArgs.Generator.Models;

namespace NavArgs.Generator;

[Generator(LanguageNames.CSharp)]
public partial class NavDestinationGenerator : IIncrementalGenerator
{

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx =>
        {
            ctx.AddSource($"{Namespace}.{NavAttributeName}.g.cs", SourceText.From(NavAttributeSourceCode, Encoding.UTF8));
            ctx.AddSource($"{Namespace}.{NavInterfaceName}.g.cs", SourceText.From(NavInterfaceSourceCode, Encoding.UTF8));
            ctx.AddSource($"{Namespace}.{ArgsInterfaceName}.g.cs", SourceText.From(ArgsInterfaceSourceCode, Encoding.UTF8));
        });
        
        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
                $"{Namespace}.{NavAttributeName}",
                static (node, _) => IsCandidate(node),
                static (context, token) =>
                {
                    var classDeclaration = GetCandidateDeclaration(context.TargetNode);

                    // if the candidate is not valid or the symbol is not valid, return
                    if (!(IsCandidateValidForCompilation(classDeclaration, context.SemanticModel) &&
                          IsCandidateSymbolValid(context.TargetSymbol)))
                    {
                        return null!; // TODO: replace for transform return type
                    }

                    token.ThrowIfCancellationRequested();

                    var navDestinationInfo = NavDestinationInfo.From((INamedTypeSymbol)context.TargetSymbol);

                    return navDestinationInfo;
                })
            .Where(static t => t != null);

        var compilation = context.CompilationProvider.Combine(provider.Collect());

        context.RegisterSourceOutput(compilation, Execute);
    }

    private static void Execute(SourceProductionContext context,
        (Compilation Left, ImmutableArray<NavDestinationInfo> Right) tuple)
    {
        var (compilation, navDestinationInfos) = tuple;

        foreach (var info in navDestinationInfos)
        {
            context.AddSource($"{info.FilenameHint}.g.cs", info.GetCompilationUnit().GetText(Encoding.UTF8));
        }
    }

    private static bool IsCandidate(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
    }

    private static ClassDeclarationSyntax GetCandidateDeclaration(SyntaxNode node) => (ClassDeclarationSyntax)node;

    private static bool IsCandidateValidForCompilation(ClassDeclarationSyntax classDeclaration,
        SemanticModel semanticModel)
    {
        if (!classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            return false;

        // Get the type symbol for the class
        ITypeSymbol? classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);

        // Get the interface symbol to check
        var navDestinationSymbol = semanticModel.Compilation.GetTypeByMetadataName($"{Namespace}.{NavInterfaceName}");

        // check if the class implements INavDestination interface 
        if (classSymbol != null && navDestinationSymbol != null)
        {
            return classSymbol.AllInterfaces.Contains(navDestinationSymbol);
            // return classSymbol.AllInterfaces.Any(t => t.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == $"{Namespace}.{InterfaceName}");
        }

        return false;
    }

    private static bool IsCandidateSymbolValid(ISymbol symbol)
    {
        return symbol is INamedTypeSymbol;
    }
}