using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class UIButtonInputAction : OnScreenControl
{
	[InputControl(layout = "Button")]
	[SerializeField] private string controlPath = "";
	[SerializeField] private Button _button;

	protected override string controlPathInternal
	{
		get => controlPath;
		set => controlPath = value;
	}

	private void Awake()
	{
		if (_button == null) _button = GetComponent<Button>();
		_button.onClick.AddListener(OnClick);
	}

	private void OnClick()
	{
		PressAsync().Forget();
	}

	private async UniTaskVoid PressAsync()
	{
		SendValueToControl(1f);
		await UniTask.NextFrame();
		SendValueToControl(0f);
	}
}
