using Cysharp.Threading.Tasks;
using S7;
using System.Collections.Generic;
using UnityEngine;

public static class SkillPreviewContextBuilder
{
    public static PresentationContext Build(PreviewUnitController character, PreviewUnitController monster)
    {
        var caster = character.View;
        var target = monster.View;

        List<UnitView> ally = new();
        ally.Add(caster);

        List<UnitView> enemys = new();
        enemys.Add(target);


        return new PresentationContext
        {
            caster = caster,
            targets = new List<UnitView>
            {
                target
            },
            onHit = (index) =>
            {
                T_EffectData effectData = T_EffectData.Get(2);
                if (effectData != null)
                {
                    EffectSpawner.Spawn(effectData, target.GetHitPoint(), target.transform).Forget();
                }
            },
            ally = ally,
            enemtys = enemys
        };
    }
}