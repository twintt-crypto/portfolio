using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPopup : UIPopupBase
{   
    protected Image _bg;
    public bool _bgClick = true;

    public Action bgClickAction = null;
    protected override void RemovePopupQueueData(string popupName)
    {
        UIPopupManager.Instance.RemovePopupQueueData(popupName);
    }

    protected override void Awake()
    {
        base.Awake();
        _bg = gameObject.GetComponent<Image>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void onPointDown(PointerEventData eventData)
    {
        base.onPointDown(eventData);
        if (_bgClick == false)
        {
            return;
        }

        if(_bg == null)
        {
            return;
        }

        if (eventData.pointerPressRaycast.gameObject == _bg.gameObject)
        {
            if(_bgClickHide == true )
            {
                if(_isHide == true)
                {
                    return;
                }

                Hide(bgClickAction);
            }
            else
            {
                bgClickAction?.Invoke();
            }            
        }        
    }

    public void SetBgClick(bool on)
    {
        if (null == _bg)
        {
            return;
        }

        _bgClick = on;
    }    
}


