using Task_3.Attributes;
using Task_3.Models;

namespace Task_3.Services;

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