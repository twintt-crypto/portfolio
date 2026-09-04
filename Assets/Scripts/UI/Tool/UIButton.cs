using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[AddComponentMenu("UI/UIButton")]
[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(Button))]
public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{
    public enum UIButtonState
    {
        Normal,
        Highlighted,
        Pressed,
        Selected,
        Disabled
    }

    private Button button;
    private Animation animation;
    public UnityAction OnClickExecute;
    public UnityAction<bool> OnPointerStateChanged;

    public bool IsPointerDown => isPointerDown;
    
    public AnimationClip NormalClip => normal;
    public AnimationClip HighlightedClip => highlighted;
    public AnimationClip PressedClip => pressed;
    public AnimationClip SelectedClip => selected;
    public AnimationClip DisabledClip => disabled;

    [Header("Button Animations")]
    [SerializeField] private AnimationClip normal;
    [SerializeField] private AnimationClip pressed;
    [SerializeField] private AnimationClip highlighted;
    [SerializeField] private AnimationClip selected;
    [SerializeField] private AnimationClip disabled;

    private UIButtonState currentState = UIButtonState.Normal;
    private UIButtonState lastState = UIButtonState.Normal;
    private bool lastInteractable = false;
    private bool isPointerOver = false;
    private bool isPointerDown = false;

    private void Awake()
    {
        if (!TryGetComponent(out button))
        {
            button = gameObject.AddComponent<Button>();
        }

        if (!TryGetComponent(out animation))
        {
            animation = gameObject.AddComponent<Animation>();
        }
       
        if (normal != null)
        {
            animation.AddClip(normal, normal.name);
        }

        if (pressed != null)
        {
            animation.AddClip(pressed, pressed.name);
        }

        if (highlighted != null)
        {
            highlighted.wrapMode = WrapMode.ClampForever;
            animation.AddClip(highlighted, highlighted.name);
        }

        if (selected != null)
        {
            selected.wrapMode = WrapMode.ClampForever;
            animation.AddClip(selected, selected.name);
        }

        if (disabled != null)
        {
            disabled.wrapMode = WrapMode.ClampForever;
            animation.AddClip(disabled, disabled.name);
        }

        button.onClick.AddListener(OnClick);
    }

    protected virtual void OnEnable()
    {
        PlayStateAnimation(button.interactable ? normal : disabled);
    }

    private void Update()
    {
        bool currentInteractable = button.interactable;
        if (currentInteractable == lastInteractable) return;

        lastInteractable = currentInteractable;
        if (!currentInteractable)
        {
            SetState(UIButtonState.Disabled, force: true);
        }
        else
        {
            SetState(isPointerOver ? UIButtonState.Highlighted : UIButtonState.Normal, force: true);
        }
    }
    
    protected virtual void OnClick()
    {
        OnClickExecute?.Invoke();
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;

        isPointerOver = true;
        SetState(UIButtonState.Highlighted);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        if (button.interactable)
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject)
                SetState(isPointerDown ? UIButtonState.Highlighted : UIButtonState.Normal);
            else
                SetState(UIButtonState.Normal);
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        if (button.interactable)
        {
            SetState(UIButtonState.Selected);
            OnPointerStateChanged?.Invoke(true);  // Down
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        if (button.interactable)
        {
            SetState(isPointerOver ? UIButtonState.Pressed : UIButtonState.Normal);
            OnPointerStateChanged?.Invoke(false);  // Up
        }
    }

    public virtual void OnSelect(BaseEventData eventData)
    {
        if (button.interactable)
            SetState(UIButtonState.Selected);
    }

    public virtual void OnDeselect(BaseEventData eventData)
    {
        if (button.interactable)
            SetState(UIButtonState.Normal);
    }
    
    private void PlayStateAnimation(AnimationClip clip)
    {
        if (clip == null || animation == null) return;

        animation.Stop();
        animation.Play(clip.name);
    }

    private void SetState(UIButtonState newState, bool force = false)
    {
        if (!force && currentState == newState)
            return;

        currentState = newState;
        AnimationClip clip = GetClipForState(currentState);
        if (clip == null || animation.IsPlaying(clip.name))
            return;
        PlayStateAnimation(clip);
    }

    private AnimationClip GetClipForState(UIButtonState state)
    {
        switch (state)
        {
            case UIButtonState.Normal: return normal;
            case UIButtonState.Highlighted: return highlighted;
            case UIButtonState.Pressed: return pressed;
            case UIButtonState.Selected: return selected;
            case UIButtonState.Disabled: return disabled;
            default: return normal;
        }
    }
    

#if UNITY_EDITOR
    public void RegisterClips()
    {
        Animation anim = GetComponent<Animation>();
        if (anim == null) return;

        TryAddClip(anim, normal);
        TryAddClip(anim, highlighted);
        TryAddClip(anim, pressed);
        TryAddClip(anim, selected);
        TryAddClip(anim, disabled);
    }

    private void TryAddClip(Animation anim, AnimationClip clip)
    {
        if (anim == null || clip == null) return;
        if (anim.GetClip(clip.name) == null)
        {
            anim.AddClip(clip, clip.name);
        }
    }
#endif
}
