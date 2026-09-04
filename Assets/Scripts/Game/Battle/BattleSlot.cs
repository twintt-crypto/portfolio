using UnityEngine;

public class BattleSlot : MonoBehaviour
{
    public int slotIndex;
    public BattleSide side; // Ally / Enemy
    public Transform standPoint; // 실제 캐릭터 서는 위치
    public Transform hitPoint;   // 피격 위치    
    public Transform targetPoint; // 포커싱
}