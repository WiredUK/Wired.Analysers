using Wired.Analysers.Test.Helpers;
using Wired.Analysers.Test.Verifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wired.Analysers.Test
{
    [TestClass]
    public class WiredAnalysersUnitTests : CodeFixVerifier
    {
        //No diagnostics expected to show up
        [TestMethod]
        public void Empty_code_returns_no_diagnostic_results()
        {
            const string test = @"";

            VerifyCSharpDiagnostic(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void Async_method_with_missing_async_suffix_returns_diagnostic_result()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
            public async Task<int> DoStuff()
            {
                return await Task.FromResult(123);
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "WiredAnalysers",
                Message = "Method name 'DoStuff' does not end with 'Async'",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 13, 36)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void Async_method_with_async_suffix_returns_no_diagnostic_result()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
            public async Task<int> DoStuffAsync()
            {
                return await Task.FromResult(123);
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "WiredAnalysers",
                Message = "Method name 'DoStuff' does not end with 'Async'",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 13, 36)
                    }
            };

            VerifyCSharpDiagnostic(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void Multiple_async_methods_with_missing_async_suffix_returns_multiple_diagnostic_results()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
            public async Task<int> DoStuff1()
            {
                return await Task.FromResult(123);
            }
            public async Task<int> DoStuff2()
            {
                return await Task.FromResult(123);
            }
        }
    }";

            var expected1 = new DiagnosticResult
            {
                Id = "WiredAnalysers",
                Message = "Method name 'DoStuff1' does not end with 'Async'",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 13, 36)
                    }
            };

            var expected2 = new DiagnosticResult
            {
                Id = "WiredAnalysers",
                Message = "Method name 'DoStuff2' does not end with 'Async'",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 17, 36)
                    }
            };

            VerifyCSharpDiagnostic(test, expected1, expected2);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new WiredAnalysersAnalyzer();
        }
    }
}
