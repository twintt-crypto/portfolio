using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace S7
{
    public class LookAtNode : BasePresentationNode
    {
        public float rotateSpeed = 10f;

        public LookAtNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
        }        

        public override async UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            var self = ctx.caster.transform;
            var target = ctx.targets[0].transform;

            RotateAsync(self, target, token).Forget();

            await UniTask.CompletedTask;
        }

        private async UniTaskVoid RotateAsync(Transform self, Transform target, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (self == null || target == null)
                    return;

                Vector3 dir = target.position - self.position;
                dir.y = 0f;

                if (dir.sqrMagnitude < 0.001f)
                    return;

                Quaternion targetRot = Quaternion.LookRotation(dir);

                self.rotation = Quaternion.Slerp(
                    self.rotation,
                    targetRot,
                    rotateSpeed * UnityEngine.Time.deltaTime
                );

                if (Quaternion.Angle(self.rotation, targetRot) < 1f)
                    return;

                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
    }
}
