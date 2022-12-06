using Task_1.Models;
using Task_1.Services;

namespace Task_1;

static class Program
{
    public static void Main(string[] args)
    {
        GeneratedVault.IVault vault = new GeneratedVault.Vault();
        // Полуение Vault
        vault = vault.GetVault(@"/Users/ginalig/Documents/TestVaultsCloudella/Vault1");
        foreach (var node in vault)
        {
            Console.WriteLine($"{node.Name}:\n{node.Text}\n");
        }
        
        // Добавление Node
        vault.AddNode(new Node("newNode", "newText"));
        foreach (var node in vault)
        {
            Console.WriteLine($"{node.Name}:\n{node.Text}\n");
        }
        
        // Нахождение Node
        var foundNode = vault.GetNode("first");
        Console.WriteLine($"{foundNode.Name}:\n{foundNode.Text}\n");
        
        // Сохранение Vault
        vault.SaveVault(@"/Users/ginalig/Documents/TestVaultsCloudella/Vault1");
    }

}