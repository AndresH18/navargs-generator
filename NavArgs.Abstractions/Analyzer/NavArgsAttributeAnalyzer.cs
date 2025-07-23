using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        context.RegisterSyntaxNodeAction(AnalyzeAttribute, SyntaxKind.Attribute);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        if (!IsValidSymbol(context))
            return;

        CheckInterface(context);
        CheckClassProperties(context);
    }

    private static void AnalyzeAttribute(SyntaxNodeAnalysisContext context)
    {
        // check if this is an attribute we care
        var attributeSyntax = (AttributeSyntax)context.Node;
        if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
            return;
        if (attributeSymbol.ContainingType.ToDisplayString() != NavAttributeFullName)
            return;

        // check if this is a constructor
        if (attributeSymbol.MethodKind != MethodKind.Constructor)
            return;

        if (attributeSyntax.ArgumentList is null)
            return;

        foreach (var argument in attributeSyntax.ArgumentList.Arguments)
        {
            // positional argument
            if (argument.NameEquals == null) continue;
            
            var constantValue = context.SemanticModel.GetConstantValue(argument.Expression);
            var name = argument.NameEquals.Name.Identifier.Text;

            if (name != "ArgsName" && name != "Route") continue;

            // if (constantValue is not { HasValue: true, Value: string or null } value)
            if (constantValue is not { HasValue: true, Value: string or null }) continue;
                
            if (constantValue.Value is string v && !string.IsNullOrWhiteSpace(v))
                continue;
                    
            var location = argument.NameEquals.Name.GetLocation();
            var diagnostic = Diagnostic.Create(DiagnosticRules.NVA003_InvalidAttributeValue, location,
                argument.NameEquals.Name, attributeSyntax.Name);
            context.ReportDiagnostic(diagnostic);
        }
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

    private static void CheckClassProperties(SymbolAnalysisContext context)
    {
        var symbol = (INamedTypeSymbol)context.Symbol;
        foreach (var property in symbol.GetMembers().OfType<IPropertySymbol>()
                     .Where(p => !p.Type.IsPrimitiveOrString()))
        {
            if (property.GetAttributes()
                .Any(a => a.AttributeClass?.ToDisplayString() == IgnoreNavPropertyAttributeFullName)
               )
                continue;

            var diagnostic = Diagnostic.Create(DiagnosticRules.NVA002_InvalidProperty,
                property.Locations[0],
                property.Name, symbol.Name, IgnoreNavPropertyAttributeFullName);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static void CheckAttributeUsage(SymbolAnalysisContext context)
    {
        var symbol = (INamedTypeSymbol)context.Symbol;
        var attribute = symbol.GetAttributes().First(a => a.AttributeClass?.ToDisplayString() == NavAttributeFullName);

        var routeArgument = attribute.NamedArguments
            .FirstOrDefault(kvp => kvp.Key == "Route").Value;

        var argsNameArgument = attribute.NamedArguments
            .FirstOrDefault(kvp => kvp.Key == "ArgsName").Value.Value?.ToString();
    }
}