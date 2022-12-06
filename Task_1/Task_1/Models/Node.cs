using Task_1.Attributes;

namespace Task_1.Models;

public class Node
{
    public Node()
    {
        Name = "";
        Text = "";
    }
    public Node(string name, string text)
    {
        if (!name.EndsWith(".node"))
        {
            name += ".node";
        }
        
        Name = name;
        Text = text;
    }
    public string Name { get; set; }
    public string Text { get; set; }
}