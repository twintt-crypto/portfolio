using System;
using System.Collections.Generic;
using UnityEngine.TextCore.Text;

namespace S7
{
    public sealed class SkillContext
    {
        // ─────────────────────────────
        // 기본 정보
        // ─────────────────────────────
        public UnitSkill SkillData { get; }
        public Character Caster { get; }

        // ─────────────────────────────
        // 타겟 정보
        // ─────────────────────────────
        public IReadOnlyList<Character> Targets { get; private set; }

        // ─────────────────────────────
        // 실행 상태 / 판정 결과
        // ─────────────────────────────
        public bool IsParried { get; private set; }
        public bool IsGuarded { get; private set; }
        public bool IsBackstab { get; private set; }

        // 멀티 히트용
        public int HitIndex { get; private set; }

        // 실행 취소 여부 (연출 캔슬 등)
        public bool IsCancelled { get; private set; }

        // ─────────────────────────────
        // Hit 콜백 (Presentation → Executor)
        // ─────────────────────────────
        private Action _onHit;

        // ─────────────────────────────
        // 생성자
        // ─────────────────────────────
        public SkillContext(
            UnitSkill skillData,
            Character caster)
        {
            SkillData = skillData;
            Caster = caster;
        }

        // ─────────────────────────────
        // 타겟 설정
        // ─────────────────────────────
        public void SetTargets(IReadOnlyList<Character> targets)
        {
            Targets = targets;
        }

        // ─────────────────────────────
        // Hit 처리
        // ─────────────────────────────
        public void BindOnHit(Action onHit)
        {
            _onHit = onHit;
        }

        public void InvokeHit()
        {
            if (IsCancelled)
                return;

            HitIndex++;
            _onHit?.Invoke();
        }

        // ─────────────────────────────
        // 판정 결과 기록
        // ─────────────────────────────
        public void MarkParried()
        {
            IsParried = true;
        }

        public void MarkGuarded()
        {
            IsGuarded = true;
        }

        public void MarkBackstab()
        {
            IsBackstab = true;
        }

        // ─────────────────────────────
        // 실행 취소
        // ─────────────────────────────
        public void Cancel()
        {
            IsCancelled = true;
        }
    }

}
