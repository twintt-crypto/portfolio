using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace S7
{
    public abstract class BasePresentationNode : IPresentationNode
    {
        protected PresentationNodeData _data;

        public RuntimeNode owner;

        public BasePresentationNode(PresentationNodeData presentationNodeData)
        {
            _data = presentationNodeData;
            //Debug.Log($"Presentation : {presentationNodeData.nodeType}");
        }

        public abstract UniTask PlayAsync(PresentationContext context, CancellationToken token);
    }

}
