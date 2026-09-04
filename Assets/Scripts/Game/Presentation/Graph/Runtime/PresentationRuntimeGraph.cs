using System.Collections.Generic;

namespace S7
{
    public class PresentationRuntimeGraph
    {
        public RuntimeNode startNode;
        public Dictionary<string, RuntimeNode> nodeMap = new();
    }
}
