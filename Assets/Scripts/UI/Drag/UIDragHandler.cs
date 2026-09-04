using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIDragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	[Header("Events")]
	[SerializeField] private UnityEvent onDragStart;
	[SerializeField] private UnityEvent<Vector2> onDragDelta;
	[SerializeField] private UnityEvent onDragEnd;

	private bool isDraging = false;

	public void OnPointerDown(PointerEventData eventData)
	{
		isDraging = true;
		
		onDragStart?.Invoke();
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (!isDraging) return;

		onDragDelta?.Invoke(eventData.delta);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (!isDraging) return;
		
		isDraging = false;
		
		onDragEnd?.Invoke();
	}
}
