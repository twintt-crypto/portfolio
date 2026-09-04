using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Analytics.IAnalytic;

namespace S7
{
    public static class DamageCalculator
    {
        private const int RATE_BASE = 1000;

        public static DamageResult Calculate(
            UnitController attacker,
            UnitController target,
            T_SkillHitData hitData)
        {
            // 1. 공격력
            int attack = attacker.data._stat.GetStat(EffectType.Attack);

            // 2. 배율 (ex: 1500 = 150%)
            int multiplier = hitData.Multiplier;

            long rawDamage = attack * multiplier / RATE_BASE;

            // 3. 방어 계산
            int defense = target.data._stat.GetStat(EffectType.Defense);

            long damageAfterDefense =
                rawDamage * RATE_BASE / (RATE_BASE + defense);

            // 4. 데미지 증/감
            damageAfterDefense = attacker.BuffManager.ApplyDamageDealtModifiers((int)damageAfterDefense);
            damageAfterDefense = target.BuffManager.ApplyDamageTakenModifiers((int)damageAfterDefense);

            // 5. 크리티컬
            bool isCritical = RollCritical(attacker);

            if (isCritical)
            {
                int critDamage = attacker.data._stat.GetStat(EffectType.CritDamage);
                damageAfterDefense = damageAfterDefense * critDamage / RATE_BASE;
            }

            // 6. 랜덤 (±5%)
            int variance = Random.Range(950, 1050); // 95% ~ 105%
            damageAfterDefense = damageAfterDefense * variance / RATE_BASE;

            // 7. 최소값
            int finalDamage = Mathf.Max(1, (int)damageAfterDefense);

            return new DamageResult
            {
                damage = finalDamage,
                isCritical = isCritical
            };
        }

        private static bool RollCritical(UnitController attacker)
        {
            int critRate = attacker.data._stat.GetStat(EffectType.CritRate); 
            return Random.Range(0, RATE_BASE) < critRate;
        }

        public static bool RollHit(UnitController attacker, UnitController target)
        {
            int accuracy = attacker.data._stat.Accuracy;   // ex: 900
            int evasion = target.data._stat.Evasion;       // ex: 200

            int finalHitRate = Mathf.Clamp(accuracy - evasion, 0, RATE_BASE);

            return Random.Range(0, RATE_BASE) <= finalHitRate;
        }
    }
}
