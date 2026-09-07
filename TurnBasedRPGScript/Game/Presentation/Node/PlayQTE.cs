using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

namespace S7
{
    public class PlayQTE : BasePresentationNode
    {
        public PlayQTE(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {            
        }

        public override async UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            await UniTask.CompletedTask;
        }        
    }
}
