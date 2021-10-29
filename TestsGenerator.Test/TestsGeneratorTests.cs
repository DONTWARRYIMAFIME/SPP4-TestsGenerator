using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using SPP4_TestsGenerator.IO;
using TestsGenerator.Lib;

namespace TestsGenerator.Test
{
    public class Tests
    {
        private CompilationUnitSyntax _class1Root;
        private CompilationUnitSyntax _class2Root;
        
        private TestsGeneratorConfig _config;
        private ITestsGenerator _generator;
        
        private const string TestFilePath = "../../../TestFile.cs";
        private const string TestClass1FilePath = "../../../../GeneratedClasses/MyTestClass1Test.cs";
        private const string TestClass2FilePath = "../../../../GeneratedClasses/MyTestClass2Test.cs";
        
        [SetUp]
        public void Setup()
        {
            var readPaths = new List<string> { TestFilePath };

            _config = new TestsGeneratorConfig(AsyncFileStream.ReadFromFile, AsyncFileStream.WriteToFile, 2, 2, 2);
            _config.ReadPaths.AddRange(readPaths);
            _generator = new NUnitTestsGenerator(_config);
            _generator.GenerateClasses();

            _class1Root = CSharpSyntaxTree.ParseText(File.ReadAllText(TestClass1FilePath)).GetCompilationUnitRoot();
            _class2Root = CSharpSyntaxTree.ParseText(File.ReadAllText(TestClass2FilePath)).GetCompilationUnitRoot();
        }

        [Test]
        public void UsingsTest()
        {
            var expected = new List<string>()
            {
                "NUnit.Framework",
                "Moq"
            };
                
            CollectionAssert.IsSubsetOf(expected, _class1Root.Usings.Select(x => x.Name.ToString()).ToList());
            CollectionAssert.IsSubsetOf(expected, _class2Root.Usings.Select(x => x.Name.ToString()).ToList());
            
            Assert.AreEqual(1, _class1Root.Usings.Count(usingEntry => usingEntry.Name.ToString() == "MyCode"));
            Assert.AreEqual(1, _class2Root.Usings.Count(usingEntry => usingEntry.Name.ToString() == "MyCode"));
        }
        
        [Test]
        public void NamespacesTest()
        {
            IEnumerable<NamespaceDeclarationSyntax> namespaces1, namespaces2;

            namespaces1 = _class1Root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
            namespaces2 = _class2Root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
            
            Assert.AreEqual(1, namespaces1.Count());
            Assert.AreEqual(1, namespaces2.Count());
            Assert.AreEqual("MyCode.Tests", namespaces1.First().Name.ToString());
            Assert.AreEqual("MyCode.Tests", namespaces2.First().Name.ToString());
        }
        
        [Test]
        public void ClassTest()
        {
            IEnumerable<ClassDeclarationSyntax> classes1, classes2;

            classes1 = _class1Root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            classes2 = _class2Root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            
            Assert.AreEqual(1, classes1.Count());
            Assert.AreEqual(1, classes2.Count());
            Assert.AreEqual("MyTestClass1Tests", classes1.First().Identifier.ToString());
            Assert.AreEqual("MyTestClass2Tests", classes2.First().Identifier.ToString());
        }
        
        [Test]
        public void MethodsTest()
        {
            var expected = new List<string>
            {
                "SetUp",
                "GetNameTest",
                "DoSumTest"
            };
            
            var actual = _class1Root.DescendantNodes().OfType<MethodDeclarationSyntax>().Select(method => method.Identifier.ToString()).ToList();
            CollectionAssert.AreEquivalent(expected, actual);
        }
        
        [Test]
        public void ClassInitTest()
        {
            var expected = 1;
            var actual = _class2Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(method => method.Identifier.ToString() == "SetUp")?.Body?.Statements
                .OfType<ExpressionStatementSyntax>()
                .Count(statement => statement.ToFullString().Contains("new MyTestClass2"));
            
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ActualTest()
        {
            var expected = 1;
            var actual = _class1Root?.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(method => method.Identifier.ToString() == "DoSumTest")?
                .Body?.Statements
                .OfType<LocalDeclarationStatementSyntax>()
                .Count(statement => statement.Declaration.Variables.Any(variable => variable.Identifier.ToString() == "actual"));
            
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ExpectedTest()
        {
            var expected = 1;
            var actual = _class1Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(method => method.Identifier.ToString() == "DoSumTest")?.Body?.Statements
                .OfType<LocalDeclarationStatementSyntax>()
                .Count(statement => statement.Declaration.Variables.Any(variable => variable.Identifier.ToString() == "expected"));
                
            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public void AreEqualTest()
        {
            var expected = 1;
            var actual = _class1Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(method => method.Identifier.ToString() == "DoSumTest")?.Body?.Statements
                .OfType<ExpressionStatementSyntax>().Count(statement => statement.ToString().Contains("Assert.AreEqual(expected, actual)"));
                
            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public void FailTest()
        {
            var expected = 1;
            var actual = _class1Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(method => method.Identifier.ToString() == "DoSumTest")?.Body?.Statements
                .OfType<ExpressionStatementSyntax>().Count(statement => statement.ToString().Contains("Assert.Fail"));
            
            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public void ArgumentsInitializationTest()
        {
            var expected = new List<string>()
            {
                "param1",
                "param2"
            };
        
            var actual = _class1Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(method => method.Identifier.ToString() == "DoSumTest")?.Body?.Statements
                .OfType<LocalDeclarationStatementSyntax>().Select(declaration => declaration.Declaration.Variables)
                .SelectMany((declaration) => declaration.ToList())
                .Select((variableDeclaration) => variableDeclaration.Identifier.ToString()).ToList();
        
            CollectionAssert.AreEquivalent(expected, actual);
        }
        
    }
}