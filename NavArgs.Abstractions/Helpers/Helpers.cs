using System.Text;
using Microsoft.CodeAnalysis;

namespace NavArgs.Abstractions.Helpers;

internal static class Helpers
{
    public static string GetFullyQualifiedMetadataName(this ITypeSymbol symbol)
    {
        var stringBuilder = new StringBuilder();
        symbol.AppendFullyQualifiedMetadataName(stringBuilder);
        return stringBuilder.ToString();
    }

    private static void AppendFullyQualifiedMetadataName(this ITypeSymbol symbol, in StringBuilder builder)
    {
        static void BuildFrom(ISymbol? symbol, in StringBuilder builder)
        {
            switch (symbol)
            {
                // Namespaces that are nested also append a leading '.'
                case INamespaceSymbol { ContainingNamespace.IsGlobalNamespace: false }:
                    BuildFrom(symbol.ContainingNamespace, in builder);
                    builder.Append('.');
                    builder.Append(symbol.MetadataName);
                    break;

                // Other namespaces (ie. the one right before global) skip the leading '.'
                case INamespaceSymbol { IsGlobalNamespace: false }:
                    builder.Append(symbol.MetadataName);
                    break;

                // Types with no namespace just have their metadata name directly written
                case ITypeSymbol { ContainingSymbol: INamespaceSymbol { IsGlobalNamespace: true } }:
                    builder.Append(symbol.MetadataName);
                    break;

                // Types with a containing non-global namespace also append a leading '.'
                case ITypeSymbol { ContainingSymbol: INamespaceSymbol namespaceSymbol }:
                    BuildFrom(namespaceSymbol, in builder);
                    builder.Append('.');
                    builder.Append(symbol.MetadataName);
                    break;

                // Nested types append a leading '+'
                case ITypeSymbol { ContainingSymbol: ITypeSymbol typeSymbol }:
                    BuildFrom(typeSymbol, in builder);
                    builder.Append('+');
                    builder.Append(symbol.MetadataName);
                    break;
                default:
                    break;
            }
        }

        BuildFrom(symbol, in builder);
    }

    public static bool IsPrimitiveOrString(this ITypeSymbol typeSymbol)
    {
        // Check for primitive types or string
        return typeSymbol.SpecialType switch
        {
            SpecialType.System_String => true,
            SpecialType.System_Boolean => true,
            SpecialType.System_Byte => true,
            SpecialType.System_SByte => true,
            SpecialType.System_Int16 => true,
            SpecialType.System_UInt16 => true,
            SpecialType.System_Int32 => true,
            SpecialType.System_UInt32 => true,
            SpecialType.System_Int64 => true,
            SpecialType.System_UInt64 => true,
            SpecialType.System_Single => true,
            SpecialType.System_Double => true,
            SpecialType.System_Decimal => true,
            SpecialType.System_Char => true,
            _ => false
        };
    }
}