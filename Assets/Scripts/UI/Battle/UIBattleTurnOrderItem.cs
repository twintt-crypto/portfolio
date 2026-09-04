using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIBattleTurnOrderItem : MonoBehaviour
{
    [SerializeField] Image _characterIcon;
    // [SerializeField] TextMeshProUGUI _nameText;

    [SerializeField] private GameObject[] _enemyObject;
    [SerializeField] private GameObject[] _playerObject;

    public BattleUnitInfo Info { get; private set; }

    public void Bind(BattleUnitInfo info)
    {
        Info = info;

        SetActiveObjects(info.isPlayer);
    }

    private void SetActiveObjects(bool isPlayer)
    {
        for (int i = 0; i < _playerObject.Length; i++) _playerObject[i].SetActive(isPlayer);
        for (int i = 0; i < _enemyObject.Length; i++) _enemyObject[i].SetActive(!isPlayer);
    }

    [EditorButton("Player 세팅")]
    private void SetPlayer()
    {
        SetActiveObjects(true);
    }

    [EditorButton("Enemy 세팅")]
    private void SetEnemy()
    {
        SetActiveObjects(false);
    }
    

    public void Remove()
    {
        Destroy(gameObject);
    }
}
