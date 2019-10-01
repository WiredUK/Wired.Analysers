using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Wired.Analysers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WiredAnalysersAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticId = "WiredAnalysers";
        private const string AsyncMethodSuffix = "Async";
        private const string Category = "Naming";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(AnalyzeMethodForAsyncSuffix, SymbolKind.Method);
        }

        private static void AnalyzeMethodForAsyncSuffix(SymbolAnalysisContext context)
        {
            var methodSymbol = (IMethodSymbol)context.Symbol;

            if (!methodSymbol.IsAsync || methodSymbol.Name.EndsWith(AsyncMethodSuffix))
            {
                // Nothing wrong with this method
                return;
            }

            var diagnostic = Diagnostic.Create(Rule, methodSymbol.Locations[0], methodSymbol.Name);

            context.ReportDiagnostic(diagnostic);
        }
    }
}
