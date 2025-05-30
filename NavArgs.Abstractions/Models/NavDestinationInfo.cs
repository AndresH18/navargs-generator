using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using NavArgs.Abstractions.Helpers;
using static NavArgs.Abstractions.Constants;

namespace NavArgs.Abstractions.Models;

internal partial record class NavDestinationInfo(
    string FilenameHint,
    string QualifiedName,
    string Namespace,
    string? Route,
    ImmutableArray<PropertyInfo> Properties)
{
    public static NavDestinationInfo From(INamedTypeSymbol symbol)
    {
        GetAttributeData(symbol, out var route);
        var name = symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var filenameHint = symbol.GetFullyQualifiedMetadataName();
        var @namespace =
            symbol.ContainingNamespace.ToDisplayString(new SymbolDisplayFormat(
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces));

        var properties = symbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.DeclaredAccessibility == Accessibility.Public)
            .Where(p => p.Name != "Route")
            .Where(p => p.Type.IsPrimitiveOrString())
            .Select(p => new PropertyInfo(p.Name, p.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)))
            .ToImmutableArray();

        return new NavDestinationInfo(filenameHint, name, @namespace, route, properties);
    }

    private static void GetAttributeData(INamedTypeSymbol symbol, out string? route)
    {
        var attribute = symbol.GetAttributes().First(a =>
            a.AttributeClass?.ToDisplayString() == $"{NavAttributeFullName}");
        // route = attribute.ConstructorArguments.First().Value?.ToString();
        route = attribute.NamedArguments
            .FirstOrDefault(kvp => kvp.Key == "Route")
            .Value.Value?.ToString();
    }
}