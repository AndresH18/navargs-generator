using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using NavArgs.Abstractions.Helpers;

namespace NavArgs.Abstractions.Models;

internal partial record NavDestinationInfo(
    string FilenameHint,
    string QualifiedName,
    string Namespace,
    string? Route,
    GenerationMode Mode,
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
            .Select(PropertyInfo.GetPropertyInfo)
            .ToImmutableArray();

        GetMetadataRoute(symbol, out var route);
        GetMetadataArgsName(symbol, out var argsName);
        GetMetadataMode(symbol, out var mode);
        return new NavDestinationInfo(filenameHint, name, @namespace, route, mode, argsName, properties);
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
            .All(a => a.AttributeClass?.ToDisplayString() != typeof(IgnoreNavPropertyAttribute).FullName);
    }


    private static void GetMetadataRoute(INamedTypeSymbol symbol, out string? route)
    {
        var attribute = symbol.GetAttributes().First(a =>
            a.AttributeClass?.ToDisplayString() == typeof(NavDestinationAttribute).FullName);
        // route = attribute.ConstructorArguments.First().Value?.ToString();
        route = attribute.NamedArguments
            .FirstOrDefault(kvp => kvp.Key == nameof(NavDestinationAttribute.Route))
            .Value.Value?.ToString();
    }

    private static void GetMetadataArgsName(INamedTypeSymbol symbol, out string? argsName)
    {
        var attribute = symbol.GetAttributes().First(a =>
            a.AttributeClass?.ToDisplayString() == typeof(NavDestinationAttribute).FullName);

        argsName = attribute.NamedArguments
            .FirstOrDefault(kvp => kvp.Key == nameof(NavDestinationAttribute.ArgsName))
            .Value.Value?.ToString();
    }

    private static void GetMetadataMode(INamedTypeSymbol symbol, out GenerationMode mode)
    {
        var attribute = symbol.GetAttributes()
            .First(a => a.AttributeClass?.ToDisplayString() == typeof(NavDestinationAttribute).FullName);

        var arg = attribute.NamedArguments.FirstOrDefault(kvp => kvp.Key == nameof(NavDestinationAttribute.Mode));
        if (arg.Key == null
            || arg.Value.Kind != TypedConstantKind.Enum
            || arg.Value.Type!.GetFullyQualifiedMetadataName() != typeof(GenerationMode).FullName)
        {
            mode = GenerationMode.Strict;
            return;
        }


        mode = (GenerationMode)arg.Value.Value!;
    }

    private string GetArgsClassName()
    {
        return string.IsNullOrWhiteSpace(ArgsName) ? $"{QualifiedName}Args" : ArgsName!;
    }
}