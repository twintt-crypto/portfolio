using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace S7
{
    public class ForkNode : BasePresentationNode
    {
        public List<RuntimeNode> children = new();

        public ForkNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
            
        }

        public override async UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (children == null || children.Count == 0)
            {
                Debug.LogWarning($"ForkNode: children is null or empty. title={_data.title}");
                return;
            }

            var tasks = new List<UniTask>(children.Count);

            foreach (var node in children)
            {
                if (node == null)
                    continue;

                tasks.Add(GraphExecutor.Execute(node, ctx, token));
            }

            if (tasks.Count == 0)
            {
                Debug.LogWarning($"ForkNode: no valid child nodes. title={_data.title}");
                return;
            }

            await UniTask.WhenAll(tasks);
        }
    }
}
