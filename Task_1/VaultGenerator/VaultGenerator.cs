using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VaultGenerator
{
    [Generator]
    public class VaultGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxTrees = context.Compilation.SyntaxTrees;
            
            // Находим интерфейс, помеченный атрибутом [Vault]
            var vaultTree = syntaxTrees.FirstOrDefault(x => x.GetText().ToString().Contains("[Vault"));
            var root = vaultTree.GetCompilationUnitRoot();
            
            // Получаем юзинги
            var usingStatements = vaultTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>()
                .Select(x => x.ToFullString());
            var usingStatementsAsText = string.Join(Environment.NewLine, usingStatements);
            
            // Получаем все методы интерфейса
            var vaultMethods = vaultTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Where(x => x.AttributeLists.Count > 0).ToList();

            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine(usingStatementsAsText);
            

            sourceBuilder.AppendLine("using System.Collections;");
            
            sourceBuilder.AppendLine("namespace GeneratedVault{\n");
            sourceBuilder.AppendLine($"\n{vaultTree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().First().ToFullString()}\n");
            sourceBuilder.AppendLine($@"public class Vault : IVault {{");
            sourceBuilder.AppendLine(Constructor());
            sourceBuilder.AppendLine(Fields());
            
            // Для каждого метода интерфейса создаем метод в классе Vault
            foreach (var vaultMethod in vaultMethods)
            {
                // Получение сигнатуры метода
                var methodName = vaultMethod.Identifier.Text;
                var returnType = vaultMethod.ReturnType.ToString();
                var parameters = vaultMethod.ParameterList.Parameters.Select(x => $"{x.Type} {x.Identifier}").ToList();
                var parametersAsText = string.Join(", ", parameters);

                sourceBuilder.AppendLine($@"public {returnType} {methodName}({parametersAsText}) {{");
                
                // Добавление тела метода
                if (vaultMethod.AttributeLists.ToFullString().Contains("GetVault"))
                {
                    sourceBuilder.AppendLine(GetVaultMethod(parameters[0].Split()[1]));
                }
                else if (vaultMethod.AttributeLists.ToFullString().Contains("AddNode"))
                {
                    sourceBuilder.AppendLine(AddNodeMethod(parameters[0].Split()[1]));
                }
                else if (vaultMethod.AttributeLists.ToFullString().Contains("SaveVault"))
                {
                    sourceBuilder.AppendLine(SaveVaultMethod(parameters[0].Split()[1]));
                }
                else if (vaultMethod.AttributeLists.ToFullString().Contains("GetNode"))
                {
                    sourceBuilder.AppendLine(GetNodeMethod(parameters[0].Split()[1]));
                }

                sourceBuilder.AppendLine("}");
            }

            sourceBuilder.AppendLine(EnumerateNodesMethod());

            sourceBuilder.AppendLine($@"}}
}}");
            context.AddSource("GeneratedVault", sourceBuilder.ToString());
        }
        
        /// <summary>
        /// Метод, который получает Vault из директории.
        /// </summary>
        /// <param name="path">Параметр метода.</param>
        /// <returns>C# код метода.</returns>
        private string GetVaultMethod(string path)
        {
            return $@"
if (!Directory.Exists({path}))
{{throw new Exception(""Directory not found"");}}
_vaultDirectory = {path};

var vault = new Vault();
var files = Directory.GetFiles({path});
var nodeFiles = files.Where(x => x.EndsWith("".node"")).ToList();
var nodeFilesContent = nodeFiles.Select(File.ReadAllText).ToList();
for (int i = 0; i < nodeFiles.Count; i++)
{{
    vault.AddNode(new Node
    {{
        Name = Path.GetFileName(nodeFiles[i]),
        Text = nodeFilesContent[i]
    }});
}}

return vault;";
        }
        
        /// <summary>
        /// Метод, который добавляет ноду в Vault.
        /// </summary>
        /// <param name="node">Параметр метода.</param>
        /// <returns>C# код метода.</returns>
        private string AddNodeMethod(string node)
        {
            return $@"
_nodes.Add({node});
File.WriteAllText(Path.Combine(_vaultDirectory, node.Name), node.Text);";
        }

        /// <summary>
        /// Метод сохранения Vault в директорию.
        /// </summary>
        /// <param name="path">Параметр метода.</param>
        /// <returns>C# код метода.</returns>
        private string SaveVaultMethod(string path)
        {
            return $@"
Directory.CreateDirectory({path});
foreach (var node in _nodes)
{{
    File.WriteAllText(Path.Combine({path}, node.Name), node.Text);
}}";
        }
        
        /// <summary>
        /// Метод, который возвращает ноду по имени.
        /// </summary>
        /// <param name="name">Параметр метода.</param>
        /// <returns>C# код метода.</returns>
        private string GetNodeMethod(string name)
        {
            return $@"
if (!{name}.EndsWith("".node""))
{{
    {name} += "".node"";
}}
return _nodes.FirstOrDefault(x => x.Name == {name}) ?? 
       throw new InvalidOperationException(""Node not found"");";
        }
        
        /// <summary>
        /// Метод итерации по нодам в Vault.
        /// </summary>
        /// <returns>C# код с методом итерации.</returns>
        private string EnumerateNodesMethod()
        {
            return $@"
public IEnumerator<Node> GetEnumerator()
{{
    return _nodes.GetEnumerator();
}}

IEnumerator IEnumerable.GetEnumerator()
{{
    return GetEnumerator();
}}";
        }
        
        /// <summary>
        /// Конструктор класса Vault.
        /// </summary>
        /// <returns>C# код с конструктором класса.</returns>
        private string Constructor()
        {
            return $@"
public Vault()
{{
    _nodes = new List<Node>();
    _vaultDirectory = """";
}}";
        }
        
        /// <summary>
        /// Поля класса Vault.
        /// </summary>
        /// <returns>C# код с полями класса.</returns>
        private string Fields()
        {
            return $@"
private readonly List<Node> _nodes;
private string _vaultDirectory;";
        }

        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif            
        }
    }
}