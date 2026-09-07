using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/UIToggleAnimated")]
[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(Toggle))]
public class UIToggleAnimated : UIToggleBase
{
    // [SerializeField] bool isUseLoopForOnClip = false;
    
    public AnimationClip NormalClip => normal;
    public AnimationClip HighlightedClip => highlighted;
    public AnimationClip PressedClip => pressed;
    public AnimationClip SelectedClip => selected;
    public AnimationClip DisabledClip => disabled;
    public AnimationClip OnClip => on;
    public AnimationClip OffClip => off;

    [Header("Toggle Animations")] 
    [SerializeField] private AnimationClip normal;
    [SerializeField] private AnimationClip highlighted;
    [SerializeField] private AnimationClip pressed;
    [SerializeField] private AnimationClip selected;
    [SerializeField] private AnimationClip disabled;
    [SerializeField] private AnimationClip on;
    [SerializeField] private AnimationClip off;

    private Animation anim;

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animation>();

        if (normal) AddClip(normal);
        if (pressed) AddClip(pressed);

        if (highlighted)
        {
            highlighted.wrapMode = WrapMode.ClampForever;
            AddClip(highlighted);
        }

        if (disabled)
        {
            disabled.wrapMode = WrapMode.ClampForever;
            AddClip(disabled);
        }

        if (on)
        {
            // on.wrapMode = (isUseLoopForOnClip ? WrapMode.Loop : WrapMode.ClampForever);
            AddClip(on);
        }

        if (off)
        {
            // off.wrapMode = WrapMode.ClampForever;
            AddClip(off);
        }
    }

    protected override void UpdateVisualState(UIToggleState state)
    {
        AnimationClip clip = state switch
        {
            UIToggleState.Normal => normal,
            UIToggleState.Highlighted => highlighted,
            UIToggleState.Pressed => pressed,
            UIToggleState.Disabled => disabled,
            UIToggleState.Selected => selected,
            UIToggleState.On => on,
            UIToggleState.Off => off,
            _ => normal,
        };

        if (clip != null && anim != null)
            anim.Play(clip.name);
    }

    private void AddClip(AnimationClip clip)
    {
        if (clip != null && anim.GetClip(clip.name) == null)
        {
            anim.AddClip(clip, clip.name);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        anim = GetComponent<Animation>();
        AddClip(normal);
        AddClip(highlighted);
        AddClip(pressed);
        AddClip(selected);
        AddClip(disabled);
        AddClip(on);
        AddClip(off);
    }
#endif
}
