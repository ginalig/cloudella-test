using Task_3.Models;

namespace Task_3;

static class Program
{
    private const string VaultPath1 = @"/Users/ginalig/Documents/TestVaultsCloudella/VaultJSON3";
    private const string VaultPath2 = @"/Users/ginalig/Documents/TestVaultsCloudella/VaultJSON4";
    public static void Main(string[] args)
    {
        GeneratedVault.IVault vault = new GeneratedVault.Vault();
        // Полуение Vault
        vault = vault.GetVault(VaultPath1);
        foreach (var node in vault)
        {
            Console.WriteLine(node.Text);
        }
        // Добавление Node
        vault.AddNode(new Node("second.node", new {text = "second"}));
        
        vault.SaveVault(VaultPath2);
    }
}