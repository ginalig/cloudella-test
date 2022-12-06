using Task_1.Attributes;
using Task_1.Models;

namespace Task_1.Services;

[Vault]
public interface IVault : IEnumerable<Node>
{
    [AddNode]
    void AddNode(Node node);

    [GetVault]
    IVault GetVault(string path);

    [GetNode]
    Node GetNode(string name);

    [SaveVault]
    void SaveVault(string path);
}