using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

namespace S7
{
    public class PlayAnimationNode : BasePresentationNode
    {
        private readonly int _animHash;

        public PlayAnimationNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
            if (presentationNodeData.param1.IsNullOrEmpty())
            {
                Debug.LogWarning("Not Animation");
            }

            _animHash = Animator.StringToHash(presentationNodeData.param1);
        }

        public override async UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            if (Animator.StringToHash("Attack_1") == _animHash)                
                SetAttackEffect(ctx);            
            
            await UniTask.Yield();

            await ctx.caster.OnlyPlayAnimationAsync(_animHash, token);
        }

        private void SetAttackEffect(PresentationContext ctx)
        {
            var receiver = ctx.caster.animationEventReceiver;
            var animator = ctx.caster.Animator;            

            receiver.RegisterAttackEffect(OnAttckEffet);

            void OnAttckEffet(int hitIndex)
            {
                Debug.Log($"OnAttckEffet : {hitIndex}");

                var hitDatas = T_SkillHitData.Get(ctx.unitSkill.skillData.TID);
                T_SkillHitData hit = hitDatas[hitIndex];

                T_EffectData effectData = T_EffectData.Get(hit.AttackEffectId);
                if(effectData == null)
                {
                    return;
                }

                EffectSpawner.Spawn(effectData, ctx.caster.transform.position, ctx.caster.transform).Forget();
            }
        }        
    }
}
