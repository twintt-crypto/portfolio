
namespace S7
{
    public interface IBuff
    {
        // 기본 정보
        int BuffId { get; }
        string Name { get; }
        EffectType EffectType { get; }
        EffectValueType EffectValueType { get; }
        // 상태
        int RemainingTurn { get; }
        int Stack { get; }

        int GetValue();

        // 버프 적용/제거
        void OnApply(UnitController owner);
        void OnRemove(UnitController owner);

        // 턴 이벤트
        void OnTurnStart(UnitController owner);
        void OnTurnEnd(UnitController owner);

        // 스택 관리
        void AddStack();
        void RefreshDuration();
        void DecreaseDuration();

        bool IsExpired();

        // 스탯 수정
        int ModifyAttack(int value);
        int ModifyDefense(int value);
        int ModifySpeed(int value);

        float ModifyCritRate(float value);
        float ModifyCritDamage(float value);

        float ModifyDamageDealt(float value);
        float ModifyDamageTaken(float value);

        // 행동 제한
        bool CanAct();
        bool CanUseSkill();
    }
}
