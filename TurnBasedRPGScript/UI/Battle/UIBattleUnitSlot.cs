using GameEventSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleUnitSlot : MonoBehaviour
{
    [Header("Unit Info")]
    // [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Image _characterImage;
    
    [Header("Unit Status")]
    [SerializeField] private Image _hpFillImage;
    [SerializeField] private TextMeshProUGUI _hpText;
    
    [Header("Active")]
    [SerializeField] private GameObject _activeObject;
    
    [Header("Ultimate")]
    [SerializeField] private Image _ultimateImage;
    [SerializeField] private GameObject[] _ultimateReady;
    [SerializeField] private GameObject[] _ultimateLoading;

    [Header("Buff")]
    [SerializeField] private Transform[] _buffSlots;
    [SerializeField] private GameObject _buffObject;
    
    [Header("Etc")]
    [SerializeField] private GameObject _deathObject;

    private void Awake()
    {
        _deathObject.SetActive(false);
        _activeObject.SetActive(false);
        OnUltimateChanged(0);
    }
    
    public BattleUnitInfo Info { get; private set; }

    public void Bind(BattleUnitInfo info)
    {
        Info = info;
        OnHpChanged(info.hp, info.maxHp);
        OnUltimateChanged(0f);

        /*EventManager.AddEventReceiver<long>(
            new EventTarget(GameEventSystem.EventType.TurnStateChange), OnTurnChanged);*/
    }

    private void OnTurnChanged(EventTarget target, long currentUnitKey)
    {
        //SetActive(Info.unitKey == currentUnitKey);
    }

    private void OnDestroy()
    {
        /*EventManager.RemoveEventReceiver<long>(
            new EventTarget(GameEventSystem.EventType.TurnStateChange), OnTurnChanged);*/
    }

    public void SetDeath()
    {
        _deathObject.SetActive(true);
    }

    public void SetActive(bool active)
    {
        _activeObject.SetActive(active);
    }

    public void OnHpChanged(int hp, int maxHp)
    {
        float ratio = maxHp > 0 ? (float)hp / maxHp : 0f;
        _hpFillImage.fillAmount = Mathf.Max(0, ratio);
        _hpText.text = $"{hp}/{maxHp}";
    }

    public void OnUltimateChanged(float ratio)
    {
        float value = Mathf.Max(0, ratio);

        if (Mathf.Abs(value - _ultimateImage.fillAmount) < 0.001f) return;
        if (_ultimateImage.fillAmount >= 1f) ActiveUltimate(false);
        
        _ultimateImage.fillAmount = Mathf.Max(0, value);

        if (_ultimateImage.fillAmount >= 1f) ActiveUltimate(true);
    }

    private void ActiveUltimate(bool active)
    {
        foreach(GameObject go in _ultimateReady)
        {
            go.SetActive(active);
        }

        foreach (GameObject go in _ultimateLoading)
        {
            go.SetActive(!active);
        }
    }

    private readonly List<GameObject> _buffInstances = new();

    public void AddBuff()
    {
        if (_buffInstances.Count >= _buffSlots.Length) return;

        Transform slot = _buffSlots[_buffInstances.Count];
        GameObject buff = Instantiate(_buffObject, slot);
        _buffInstances.Add(buff);
    }

    public void RemoveBuff()
    {
        if (_buffInstances.Count == 0) return;

        GameObject last = _buffInstances[_buffInstances.Count - 1];
        _buffInstances.RemoveAt(_buffInstances.Count - 1);
        Destroy(last);
    }

    public void ClearBuffs()
    {
        foreach (GameObject buff in _buffInstances)
        {
            Destroy(buff);
        }
        _buffInstances.Clear();
    }

#if UNITY_EDITOR
    [ContextMenu("Test HP 100%")]
    private void TestHpFull() => OnHpChanged(100, 100);
    [ContextMenu("Test HP 50%")]
    private void TestHpHalf() => OnHpChanged(50, 100);
    [ContextMenu("Test HP 0%")]
    private void TestHpZero() => OnHpChanged(0, 100);
    [ContextMenu("Test Ultimate 100%")]
    private void TestUltimateFull() => OnUltimateChanged(1f);
    [ContextMenu("Test Ultimate 50%")]
    private void TestUltimateHalf() => OnUltimateChanged(0.5f);
    [ContextMenu("Test Ultimate 0%")]
    private void TestUltimateZero() => OnUltimateChanged(0f);
    [ContextMenu("Test Death")]
    private void TestDeath() => SetDeath();
    [ContextMenu("Test Active On")]
    private void TestActiveOn() => SetActive(true);
    [ContextMenu("Test Active Off")]
    private void TestActiveOff() => SetActive(false);
    [ContextMenu("Test Buff Add")]
    private void TestBuffAdd() => AddBuff();
    [ContextMenu("Test Buff Remove")]
    private void TestBuffRemove() => RemoveBuff();
    [ContextMenu("Test Buff Clear")]
    private void TestBuffClear() => ClearBuffs();
#endif
}
