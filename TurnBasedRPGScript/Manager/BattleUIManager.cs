using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleUIManager : Singleton<BattleUIManager>
{
    [SerializeField] Transform battleCanvas;

    public Transform GetCanvas()
    {
        return battleCanvas;
    }

    public async UniTask ShowDamage(Vector3 worldPos, int damage)
    {
       var obj = await ResourceManager.NewAsync("DamageText", battleCanvas, true);       

        RectTransform rect = obj.GetComponent<RectTransform>();

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        rect.position = screenPos;

        obj.GetComponent<DamageText>().Play(damage);
    }
}
