using Task_2.Attributes;
using Task_2.Models;

namespace Task_2.Services;

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