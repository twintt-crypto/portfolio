#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[AddComponentMenu("UI/UISoundButton")]
[RequireComponent(typeof(Button))]
public class UISoundButton : MonoBehaviour
{
    public enum eUIButtonSound
    {
        None = 0,
        
        // UI sound
        Normal =	218,
        Select =	215,
        BackButton =	223,
        TabChange_Main =	213,
        TabChange_Sub = 214,
        Slidebar = 224,
    }

    [SerializeField] private eUIButtonSound _clickSound = eUIButtonSound.Normal;
    private Button _button;

    private void Awake()
    {
        if (!Application.isPlaying) return;
        if (!_button) _button = GetComponent<Button>();
        if (_button == null) return;

        // 직렬화된 이벤트가 없을 때만 동적 등록
        if (!HasPersistentListener(_button, PlayButtonSound))
        {
            _button.onClick.RemoveListener(PlayButtonSound);
            _button.onClick.AddListener(PlayButtonSound);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!_button) _button = GetComponent<Button>();
        if (_button == null) return;

        // Editor 단계에서 항상 Persistent Listener로 동기화
        UnityEventTools.RemovePersistentListener(_button.onClick, PlayButtonSound);
        UnityEventTools.AddPersistentListener(_button.onClick, PlayButtonSound);
        EditorUtility.SetDirty(_button);
    }

    private void OnDestroy()
    {
        if (_button == null) return;

        // Prefab/씬 양쪽에서 컴포넌트 제거 시 이벤트 정리
        UnityEventTools.RemovePersistentListener(_button.onClick, PlayButtonSound);
        EditorUtility.SetDirty(_button);
    }
#endif

    private void PlayButtonSound()
    {
        if (_clickSound != eUIButtonSound.None)
        {
            // SoundManager.Singleton.PlaySfxWithIndex((int)_clickSound);
        }
    }

    /// <summary> 현재 버튼에 해당 메서드가 PersistentListener로 등록되어 있는지 확인 </summary>
    private bool HasPersistentListener(Button button, UnityAction call)
    {
        var ev = button.onClick;
        for (int i = 0; i < ev.GetPersistentEventCount(); i++)
        {
            if (ev.GetPersistentTarget(i) == this &&
                ev.GetPersistentMethodName(i) == call.Method.Name)
            {
                return true;
            }
        }
        return false;
    }
}
