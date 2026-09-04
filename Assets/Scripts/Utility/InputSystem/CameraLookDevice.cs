using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
#endif
[InputControlLayout(displayName = "Camera Look Device")]
public class CameraLookDevice : InputDevice
{
    [InputControl(layout = "Vector2")]
    public Vector2Control look { get; private set; }

    protected override void FinishSetup()
    {
        base.FinishSetup();
        look = GetChildControl<Vector2Control>("look");
    }

    static CameraLookDevice()
    {
        InputSystem.RegisterLayout<CameraLookDevice>();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        InputSystem.RegisterLayout<CameraLookDevice>();
    }
}
