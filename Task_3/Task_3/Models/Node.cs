using System.Text.Json;
using Task_3.Attributes;

namespace Task_3.Models;

public class Node
{
    public Node()
    {
        Name = "";
        Obj = new();
    }
    public Node(string name, string text)
    {
        if (!name.EndsWith(".node"))
        {
            name += ".node";
        }

        Name = name;
        Obj = JsonSerializer.Deserialize<Object>(text) ?? throw new InvalidOperationException("Invalid JSON");
    }
    
    public Node(string name, Object obj)
    {
        if (!name.EndsWith(".node"))
        {
            name += ".node";
        }

        Name = name;
        Obj = obj;
    }
    public string Name { get; init; }
    public string Text => JsonSerializer.Serialize(Obj);
    
    public Object Obj { get; init; }
}