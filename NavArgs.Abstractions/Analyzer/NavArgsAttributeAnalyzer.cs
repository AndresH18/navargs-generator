using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NavArgs.Abstractions.Helpers;
using static NavArgs.Abstractions.Constants;

namespace NavArgs.Abstractions.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NavArgsAttributeAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = DiagnosticRules.Rules;

    public override void Initialize(AnalysisContext context)
    {
        // avoid analyzing generated code.
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        // enable the Concurrent Execution.
        context.EnableConcurrentExecution();

        // subscribe to 'SyntaxKind' (ClassDeclaration) action.
        // context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.ClassDeclaration);

        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    private void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        if (!IsValidSymbol(context))
            return;

        CheckInterface(context);
        CheckProperties(context);
    }

    private static bool IsValidSymbol(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol symbol)
            return false;

        if (symbol.GetAttributes().All(a => a.AttributeClass?.ToDisplayString() != NavAttributeFullName))
            return false;

        return true;
    }

    private static void CheckInterface(SymbolAnalysisContext context)
    {
        var symbol = (INamedTypeSymbol)context.Symbol;

        var navDestinationInterface = context.Compilation.GetTypeByMetadataName(NavInterfaceFullName);
        if (navDestinationInterface == null) return;

        if (symbol.AllInterfaces.Contains(navDestinationInterface))
            return;

        var diagnostic = Diagnostic.Create(DiagnosticRules.NVA001_InterfaceMissing, symbol.Locations[0], symbol.Name,
            navDestinationInterface.Name);
        context.ReportDiagnostic(diagnostic);
    }

    private static void CheckProperties(SymbolAnalysisContext context)
    {
        var symbol = (INamedTypeSymbol)context.Symbol;
        foreach (var property in symbol.GetMembers().OfType<IPropertySymbol>()
                     .Where(p => !p.Type.IsPrimitiveOrString()))
        {
            if (property.GetAttributes()
                .Any(a => a.AttributeClass?.ToDisplayString() == IgnoreNavPropertyAttributeFullName)
               ) continue;

            var diagnostic = Diagnostic.Create(DiagnosticRules.NVA002_InvalidProperty,
                property.Locations[0],
                property.Name, symbol.Name, IgnoreNavPropertyAttributeFullName);
            context.ReportDiagnostic(diagnostic);
        }
    }
}