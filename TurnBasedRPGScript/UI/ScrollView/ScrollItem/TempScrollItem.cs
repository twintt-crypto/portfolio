using Gpm.Ui;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TempData : InfiniteScrollData
{
    public int index = 0;    

}

public class TempScrollItem : BaseScrollItem<TempData>
{
    SwipeInfiniteScroll scrollView = null;

    [SerializeField] RectTransform _content;

    protected override void UpdateData(TempData data)
    {
        scrollView = scroll as SwipeInfiniteScroll;
    }

    private void FixedUpdate()
    {
        if (scrollView == null)
        {
            return;
        }

        if (scrollView.value == 0)
        {
            return;
        }

        float distance = Vector2.Distance(new Vector2(_content.transform.position.x, 0), new Vector2(transform.parent.parent.parent.position.x, 0));

        // 거리에 따른 스케일 계산
        float scaleFactor = Mathf.Clamp(1 - (distance * scrollView.value), scrollView.minScale, scrollView.maxScale);

        // 객체의 스케일 조정
        _content.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }
}
