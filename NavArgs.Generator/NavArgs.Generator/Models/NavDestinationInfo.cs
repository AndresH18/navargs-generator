using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using NavArgs.Generator.Helpers;

namespace NavArgs.Generator.Models;

internal partial record class NavDestinationInfo(
    string FilenameHint,
    string QualifiedName,
    string Namespace,
    ImmutableArray<PropertyInfo> Properties)
{
    public static NavDestinationInfo From(INamedTypeSymbol symbol)
    {
        var name = symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var filenameHint = symbol.GetFullyQualifiedMetadataName();
        var @namespace =
            symbol.ContainingNamespace.ToDisplayString(new SymbolDisplayFormat(
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces));

        var properties = symbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.DeclaredAccessibility == Accessibility.Public)
            .Where(p => p.Type.IsPrimitiveOrString())
            .Select(p => new PropertyInfo(p.Name, p.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)))
            .ToImmutableArray();

        return new NavDestinationInfo(filenameHint, name, @namespace, properties);
    }
}