using S7;
using Unity.Cinemachine;
using UnityEngine;

public class UnitUI : MonoBehaviour
{
    [SerializeField] private Nameplate _nameplate;
    [SerializeField] private Transform _topPoint;
    [SerializeField] private Transform _hitPoint;

    [SerializeField] private Transform _targetting;

    public Vector3 offset = new Vector3(0, 0.5f, 0);

    public Transform TopPoint { get => _topPoint; }
    public Nameplate Nameplate { get => _nameplate; }    

    void Awake()
    {

    }

    private void OnEnable()
    {
        UIManager.Instance.RegisterUnitUi(this);
    }
    private void OnDisable()
    {
        UIManager.Instance.UnRegisterUnitUi(this);
    }    

    public void SetDamage(long damage)
    {

    }   

    public void SetTargetiing(bool on, Transform hitPoint)
    {
        _targetting.SetActive(on);        
    }

    private void LateUpdate()
    {
        if(_targetting.gameObject.activeSelf == true)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(_hitPoint.position);
            _targetting.transform.position = screenPos;
        }
    }
}