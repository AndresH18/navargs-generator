using Microsoft.CodeAnalysis;

namespace NavArgs.Abstractions.Models;

internal record PropertyInfo(
    string Name,
    string Type,
    bool IsNullable,
    bool IsValueType)
{
    private static readonly SymbolDisplayFormat DisplayFormat = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
                              SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
    // public string BaseType => Type.Replace("?", "");

    // public string FormattedType => IsSimpleType ? Type : BaseType;

    public string TypeCast => Type;

    /*
                 Cast | As
        int       X
        int?      X     X
        Guid      X
        Guid?     X     X

        string    X     X
        string?   X
        object    X     X
        object?   X
     */
    public string TypeAs
    {
        get
        {
            if (IsValueType == IsNullable) // IsValueType && IsNullable || !IsValueType && !IsNullable
                return Type;

            return Type.Replace("?", "");
        }
    }

    public string RawType => Type.Replace("?", "");

    public static PropertyInfo GetPropertyInfo(IPropertySymbol property)
    {
        var name = property.Name;
        var type = property.Type.ToDisplayString(DisplayFormat);
        var isNullable = property.Type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T ||
                         property.Type is { IsReferenceType: true, NullableAnnotation: NullableAnnotation.Annotated };

        var isValueType = property.Type.IsValueType ||
                          property.Type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;

        return new PropertyInfo(name, type, isNullable, isValueType);
    }
}