using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace S7
{
    public class JoinNode : BasePresentationNode
    {
        public int requiredCount;

        private int _arrivedCount;
        private UniTaskCompletionSource _tcs;
        private bool _completed;

        public JoinNode(PresentationNodeData data) : base(data)
        {
            ResetState();
        }

        public override async UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            _arrivedCount++;

            bool isLast = false;

            if (_arrivedCount >= requiredCount && !_completed)
            {
                _completed = true;
                isLast = true;
                _tcs.TrySetResult();
            }

            try
            {
                await _tcs.Task.AttachExternalCancellation(token);
            }
            catch
            {
                return;
            }

            // 여기 핵심
            if (!isLast)
                return;

            // 마지막 도착자만 다음 실행
            if (owner.nextNodes == null || owner.nextNodes.Count == 0)
                return;

            if (owner.nextNodes.Count == 1)
            {
                await GraphExecutor.Execute(owner.nextNodes[0], ctx, token);
                return;
            }

#if UNITY_EDITOR
            UnityEngine.Debug.LogError($"JoinNode nextNodes 이상 guid={owner.guid}");
#endif
        }

        public void ResetState()
        {
            _arrivedCount = 0;
            _completed = false;
            _tcs = new UniTaskCompletionSource();
        }
    }
}
