using System.Collections.Generic;

namespace S7
{
    public class RuntimeNode
    {
        public string guid;
        public IPresentationNode node;

        public List<RuntimeNode> nextNodes = new();
        public List<RuntimeNode> prevNodes = new();
    }
}
