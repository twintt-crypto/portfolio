using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

public class InputCameraLook : OnScreenControl
{
	[InputControl(layout = "Vector2")]
	[SerializeField] private string controlPath = "";
	[SerializeField] private float sensitivity = 1f;

	protected override string controlPathInternal
	{
		get => controlPath;
		set => controlPath = value;
	}

	private bool isDragging;
	private Vector2 frameDelta;

	public void BeginDrag()
	{
		isDragging = true;
	}

	public void SetInput(Vector2 value)
	{
		frameDelta += value;
	}

	public void EndDrag()
	{
		isDragging = false;
		SendValueToControl(Vector2.zero);
	}

	private void LateUpdate()
	{
		if (!isDragging) return;

		SendValueToControl(frameDelta * sensitivity);
		frameDelta = Vector2.zero;
	}
}
