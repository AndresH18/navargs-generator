using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using NavArgs.Abstractions.Helpers;
using static NavArgs.Abstractions.Constants;

namespace NavArgs.Abstractions.Models;

internal partial record NavDestinationInfo(
    string FilenameHint,
    string QualifiedName,
    string Namespace,
    string? Route,
    string? ArgsName,
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
            .Where(ShouldIncludeProperty)
            .Select(GetPropertyInfo)
            .ToImmutableArray();

        GetMetadataRoute(symbol, out var route);
        GetMetadataArgsName(symbol, out var argsName);
        return new NavDestinationInfo(filenameHint, name, @namespace, route, argsName, properties);
    }

    private static bool ShouldIncludeProperty(IPropertySymbol property)
    {
        if (property.DeclaredAccessibility != Accessibility.Public)
            return false;

        if (property.Name == "Route")
            return false;

        // return property.Type.IsPrimitiveOrString();
        if (property.GetAttributes().Length == 0)
            return true;

        return property.GetAttributes()
            .All(a => a.AttributeClass?.ToDisplayString() != IgnoreNavPropertyAttributeFullName);
    }

    private static PropertyInfo GetPropertyInfo(IPropertySymbol property)
    {
        var name = property.Name;
        var type = property.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).Trim();
        var isNullable = property.Type.NullableAnnotation == NullableAnnotation.Annotated;
        var isSimple = property.Type.IsPrimitiveOrString();
        return new PropertyInfo(name, type, isNullable, isSimple);
    }

    private static void GetMetadataRoute(INamedTypeSymbol symbol, out string? route)
    {
        var attribute = symbol.GetAttributes().First(a =>
            a.AttributeClass?.ToDisplayString() == NavAttributeFullName);
        // route = attribute.ConstructorArguments.First().Value?.ToString();
        route = attribute.NamedArguments
            .FirstOrDefault(kvp => kvp.Key == "Route")
            .Value.Value?.ToString();
    }

    private static void GetMetadataArgsName(INamedTypeSymbol symbol, out string? argsName)
    {
        var attribute = symbol.GetAttributes().First(a =>
            a.AttributeClass?.ToDisplayString() == NavAttributeFullName);

        argsName = attribute.NamedArguments
            .FirstOrDefault(kvp => kvp.Key == "ArgsName")
            .Value.Value?.ToString();
    }

    private string GetArgsClassName()
    {
        return string.IsNullOrWhiteSpace(ArgsName) ? $"{QualifiedName}Args" : ArgsName!;
    }
}