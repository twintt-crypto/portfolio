using Cysharp.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using UnityEngine;
using GameEventSystem;

namespace S7
{
    public partial class SkillManager
    {
        private BattleContext _context;

        private SkillResult _result;

        private IEnumerable<UnitController> _allUnits;
        private readonly PresentationCore _presentationCore;

        public SkillManager(PresentationCore presentationCore)
        {
            _presentationCore = presentationCore;
        }

        public void Initialize(BattleContext battleContext, IEnumerable<UnitController> units)
        {
            _context = battleContext;
            _allUnits = units;
            InitializeSkillAction(units);
        }

        public async UniTask ExecuteSkill(ActionContext actionContext, CancellationToken token)
        {
            if (actionContext.unitSkill.skillData == null)
            {
                return;
            }

            _result = Executer(actionContext);
            if (_result == null)
            {
                return;
            }

            PresentationContext context = new PresentationContext();
            context.unitSkill = actionContext.unitSkill;
            context.result = _result;
            context.caster = actionContext.caster.view;
            context.targets = new List<UnitView>();


            foreach (var unit in actionContext.targets)
            {
                context.targets.Add(unit.view);
            }

            context.ally.Clear();
            context.enemtys.Clear();

            foreach (var unit in _allUnits)
            {
                if(unit.data.unitType == UnitType.Character)
                {
                    context.ally.Add(unit.view);
                }
                else if(unit.data.unitType == UnitType.Monster)
                {
                    context.enemtys.Add(unit.view);
                }
            }            

            context.onHit = (hitindex) => ApplyResult(_result, hitindex);

            T_ProjectileData projectileData = T_ProjectileData.Get(actionContext.unitSkill.skillData.ProjectileId);
            if (projectileData != null)
            {
                await ObjectPoolManager.Instance.PreLoadAsync(projectileData.Prefab, 5);
            }

            await _presentationCore.PlayAsync(context.unitSkill.skillData.PresentationGraph, context, token);
            
            //얀츨 끝나고 ap 수정
            _context.ap += _result.ap;
            EventManager.BroadCasting(new EventTarget(GameEventSystem.EventType.UpdateAp), _context.ap);

        }

        public async UniTask UseSkill(UnitController caster, UnitSkill unitSkill, List<UnitController> targets, CancellationToken token)
        {
            var ctx = new ActionContext(
                caster,
                unitSkill,
                targets);


            await ExecuteSkill(ctx, token);
        }

        public async UniTask UseSkill(UnitController caster, UnitSkill unitSkill, UnitController target, CancellationToken token)
        {
            var ctx = new ActionContext(
                caster,
                unitSkill,
                target);

            await ExecuteSkill(ctx, token);
        }


        public async UniTask UseSkill(UnitController caster, UnitSkill unitSkill, CancellationToken token)
        {
            var ctx = new ActionContext(caster, unitSkill);


            await ExecuteSkill(ctx, token);
        }

        public async UniTask CheckActivateSkill(IEnumerable<UnitController> units, ActivationCondition condition, CancellationToken token)
        {
            foreach (var iter in units)
            {
                foreach (var skill in iter.skills)
                {
                    if (skill.Value.skillData == null)
                    {
                        continue;
                    }

                    if (skill.Value.skillData.ActivationCondition != condition)
                    {
                        continue;
                    }

                    await UseSkill(iter, skill.Value, token);
                }
            }
        }

        // onHit결과`
        private void ApplyResult(SkillResult result, int hitIndex)
        {            
            if (result == null)
                return;

            ApplyResultCore(result, hitIndex);
            PlayHitFeedback(result, hitIndex);
        }        

        private void ApplyResultCore(SkillResult result, int hitIndex)
        {
            if (result == null)
                return;

            var results = result.hitResults.Where((x) => x.hitIndex == hitIndex);
            foreach(var hit in results)
            {
                hit.hitTime = Time.unscaledTime;

                //ApplyEffect(hit.caster, hit.target, result.skillData.EffectId)
                //item.
                //item.target.ApplyDamage(item.damage);
            }

            // 2. 브레이크 즉시 반영
            /*if (hitResult.breakDamage > 0)
            {
                target.ApplyBreak(hitResult.breakDamage);
            }*/

            // 3. effect 즉시 반영
            /*foreach (var effectResult in result.buffResults)
            {                
                ApplyEffect(effectResult);
            }*/
        }

        private void ApplyEffect(UnitController caster, UnitController target, T_SkillEffectData effectData )
        {
            
        }

        private void PlayHitFeedback(SkillResult result, int hitIndex)
        {
            if (result == null)
                return;

            var results = result.hitResults.Where((x) => x.hitIndex == hitIndex);
            foreach (var hit in results)
            {
                if (hit.damage > 0)
                {                    
                    hit.target.view.OnHit(hit, T_SkillHitData.Get(result.skillData.TID)).Forget();                    
                }
            }            
        }
    }
}
