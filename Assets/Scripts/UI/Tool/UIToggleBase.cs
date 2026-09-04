using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class UIToggleBase : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler, 
    IPointerDownHandler, IPointerUpHandler,
    ISelectHandler, IDeselectHandler
{
    public enum UIToggleState
    {
        Normal,
        Highlighted,
        Pressed,
        Disabled,
        Selected,
        On,
        Off
    }

    [Header("Toggle Objects")]
    [SerializeField] protected List<GameObject> onObjects = new();
    [SerializeField] protected List<GameObject> offObjects = new();

    [Header("Setting")] 
    [SerializeField] protected bool isUsePressedAnimation = false;
    [SerializeField] protected bool isUseSelectedAnimation = false;

    protected Toggle toggle;
    protected UIToggleState currentState = UIToggleState.Normal;
    protected bool lastInteractable = false;

    protected virtual void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    protected virtual void OnEnable()
    {
        UpdateVisualState(toggle.interactable ? (toggle.isOn ? UIToggleState.On : UIToggleState.Off) : UIToggleState.Disabled);
        ApplyOnOffObjects(toggle.isOn);
    }

    protected virtual void Update()
    {
        if (toggle.interactable != lastInteractable)
        {
            lastInteractable = toggle.interactable;
            UpdateVisualState(toggle.interactable ? (toggle.isOn ? UIToggleState.On : UIToggleState.Normal) : UIToggleState.Disabled);
        }
    }

    private void OnToggleValueChanged(bool isOn)
    {
        UpdateVisualState(isOn ? UIToggleState.On : UIToggleState.Off);
        ApplyOnOffObjects(isOn);
    }

    protected void ApplyOnOffObjects(bool isOn)
    {
        foreach (var obj in onObjects)
            if (obj != null) obj.SetActive(isOn);
        foreach (var obj in offObjects)
            if (obj != null) obj.SetActive(!isOn);
    }

    protected abstract void UpdateVisualState(UIToggleState state);

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (toggle.interactable)
            UpdateVisualState(toggle.isOn ? UIToggleState.On : UIToggleState.Highlighted);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (toggle.interactable)
            UpdateVisualState(toggle.isOn ? UIToggleState.On : UIToggleState.Off);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (toggle.interactable && isUsePressedAnimation)
            UpdateVisualState(UIToggleState.Pressed);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        if (toggle.interactable && isUseSelectedAnimation)
            UpdateVisualState(UIToggleState.Selected);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (toggle.interactable && isUseSelectedAnimation)
            UpdateVisualState(UIToggleState.Normal);
    }
}
