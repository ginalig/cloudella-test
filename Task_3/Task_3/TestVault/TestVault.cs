using Task_3.Attributes;

using Task_3.Models;

using System.Collections;
namespace Task_3.TestVault{



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

    public class Vault : IVault {

        public Vault()
        {
            _nodes = new List<Node>();
            _vaultDirectory = "";
        }

        private readonly List<Node> _nodes;
        private string _vaultDirectory;
        public void AddNode(Node node) {

            _nodes.Add(node);
            File.WriteAllText(Path.Combine(_vaultDirectory, node.Name), node.Text);
        }
        public IVault GetVault(string path) {

            if (!Directory.Exists(path))
            {
                throw new Exception("Directory not found");
            }
            _vaultDirectory = path;

            var vault = new Vault();
            var files = Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith(".node"))
                {
                    vault.AddNode(new Node(Path.GetFileName(files[i]), File.ReadAllText(files[i])));
                }
            }

            return vault;
        }
        public Node GetNode(string name) {

            if (!name.EndsWith(".node"))
            {
                name += ".node";
            }

            for (int i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i].Name == name)
                {
                    return _nodes[i];
                }
            }
            throw new Exception("Node not found");
        }
        public void SaveVault(string path) {

            Directory.CreateDirectory(path);
            foreach (var node in _nodes)
            {
                File.WriteAllText(Path.Combine(path, node.Name), node.Text);
            }
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}