using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NavArgs.Abstractions.Models;

internal record PropertyInfo(
    string Name,
    string TypeName,
    bool IsNullable,
    bool IsSimple)
{
    internal string GetValidTypeName()
    {
        /*
                    Not Nullable    |   Nullable
            Simple      Keep            Keep
            Complex     Null        |   Keep
         */

        if (IsNullable || IsSimple)
            return TypeName;

        return $"{TypeName}?";
    }

    internal string NormalizedType => TypeName.Replace("?", "");
    //
    internal bool ShouldBeNullable => IsNullable || !IsSimple;

    internal TypeSyntax GetTypeSyntax()
    {
        if (IsSimple && IsNullable)
            return ParseTypeName(TypeName);

        return ParseTypeName(TypeName.Replace("?", ""));
    }
}