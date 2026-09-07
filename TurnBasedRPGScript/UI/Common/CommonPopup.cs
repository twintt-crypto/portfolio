using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PopupType
{
    Ok,
    Cancel,
    OkCancel,
    noButton,
}

public static class CommonPopup
{
    public static async UniTask PopupMessage(string popupName, string Title, string notice, POPUP_TYPE type, Action funcOk = null, Action funcCancel = null)
    {
        var popup = await UIPopupManager.Instance.ShowPopupAsync("UIPopupMessage", popupName) as UIPopupMessage;
        if(popup == null)
        {
            return;
        }

        popup.SetData(Title, notice, type, funcOk, funcCancel);
        popup.SetBtnTextOK(StringManager.Get("UI_OK"));
        popup.SetBtnTextCancel(StringManager.Get("UI_CANCEL"));        
    }

    public static async UniTask PopupTimerMessage(string popupName, string Title, string notice, float time, Action func = null)
    {
        var popup =await UIPopupManager.Instance.ShowPopupAsync("UIPopupMessage", popupName) as UIPopupMessage;
        if (popup == null)
        {
            return;
        }

        popup.SetData(Title, notice, POPUP_TYPE.NO_BUTTON);
        popup.SetTime(time, func);            
    }    
}


   
