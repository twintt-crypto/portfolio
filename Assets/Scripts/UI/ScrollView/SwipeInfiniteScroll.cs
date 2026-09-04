using Gpm.Ui;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeInfiniteScroll : InfiniteScroll , IBeginDragHandler, IEndDragHandler, IDragHandler
{
    bool isDrag = false;
    float velocity = 1000f;

    bool isMove = false;

    public float value = 1;
    public float maxScale = 1f; // 최대 스케일
    public float minScale = 0.7f; // 최소 스케일

    public Action<int> onSelect;

    public virtual void Start()
    {
        
    }    

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        do
        {
            if (isMove == true) break;            
            if (isDrag == true) break;

            if(scrollRect.vertical == false && scrollRect.horizontal == false)
            {
                break;
            }

            if (scrollRect.velocity.magnitude < velocity && scrollRect.velocity.magnitude != 0)
            {
                int selectIndex = GetSelectItemIndex();                
                if(selectIndex == -1) break;

                scrollRect.velocity = Vector2.zero;
                isMove = true;                
                
                MoveTo(selectIndex, MoveToType.MOVE_TO_CENTER, 0.2f);
                //Debug.Log($"MoveTo : {selectIndex}");
                CommonUtil.DelayedCall(0.2f, () =>
                {                    
                    scrollRect.velocity = Vector2.zero;
                    isMove = false;
                    onSelect?.Invoke(selectIndex);
                });
            }
        } while (false);        
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;        
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;        
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        
    }
    public virtual void OnSelect()
    {

    }

    public int GetSelectItemIndex()
    {
        var delta = GetItemCount() - 1;
        if (delta <= 0)
            return 0;

        float cal = 1.0f / delta;

        float position = 0;
        if(scrollRect.vertical == true)
        {
            position = 1 - scrollRect.verticalNormalizedPosition;
        }
        else if(scrollRect.horizontal == true)
        {
            position = scrollRect.horizontalNormalizedPosition;
        }
        else
        {
            return 0;
        }

        return (int)((position + (cal / 2)) / cal);
    }    

    public T GetData<T>() where T : InfiniteScrollData
    {                
        return GetData(GetSelectItemIndex()) as T;
    }
}
