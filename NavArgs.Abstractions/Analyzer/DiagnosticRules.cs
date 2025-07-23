using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

// ReSharper disable InconsistentNaming

namespace NavArgs.Abstractions.Analyzer;

internal static class DiagnosticRules
{
    public static ImmutableArray<DiagnosticDescriptor> Rules => ImmutableArray.Create(
        NVA001_InterfaceMissing,
        NVA002_InvalidProperty,
        NVA003_InvalidAttributeValue
    );

    // NVA001

    public const string NVA001_Id = "NVA001";

    public static readonly LocalizableString NVA001_Title =
        new LocalizableResourceString(nameof(Resources.NVA001Title), Resources.ResourceManager, typeof(Resources));

    public static readonly LocalizableString NVA001_Description =
        new LocalizableResourceString(nameof(Resources.NVA001Description), Resources.ResourceManager,
            typeof(Resources));

    public static readonly LocalizableString NVA001_MessageFormat =
        new LocalizableResourceString(nameof(Resources.NVA001MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    public static readonly DiagnosticDescriptor NVA001_InterfaceMissing =
        new(NVA001_Id, NVA001_Title, NVA001_MessageFormat, "Usage",
            DiagnosticSeverity.Warning, isEnabledByDefault: true, description: NVA001_Description);


    // NVA002

    public const string NVA002_Id = "NVA002";

    public static readonly LocalizableString NVA002_Title =
        new LocalizableResourceString(nameof(Resources.NVA002Title), Resources.ResourceManager, typeof(Resources));

    public static readonly LocalizableString NVA002_Description =
        new LocalizableResourceString(nameof(Resources.NVA002Description), Resources.ResourceManager,
            typeof(Resources));

    public static readonly LocalizableString NVA002_MessageFormat =
        new LocalizableResourceString(nameof(Resources.NVA002MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    public static readonly DiagnosticDescriptor NVA002_InvalidProperty =
        new(NVA002_Id, NVA002_Title, NVA002_MessageFormat, "Usage",
            DiagnosticSeverity.Error, isEnabledByDefault: true, description: NVA002_Description);
    
    // NVA003
    public const string NVA003_Id = "NVA003";
    public static readonly LocalizableString NVA003_Title =
        new LocalizableResourceString(nameof(Resources.NVA003Title), Resources.ResourceManager, typeof(Resources));

    public static readonly LocalizableString NVA003_Description =
        new LocalizableResourceString(nameof(Resources.NVA003Description), Resources.ResourceManager,
            typeof(Resources));

    public static readonly LocalizableString NVA003_MessageFormat =
        new LocalizableResourceString(nameof(Resources.NVA003MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    public static readonly DiagnosticDescriptor NVA003_InvalidAttributeValue =
        new(NVA003_Id, NVA003_Title, NVA003_MessageFormat, "Usage",
            DiagnosticSeverity.Error, isEnabledByDefault: true, description: NVA002_Description);

}