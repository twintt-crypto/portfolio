using DG.Tweening;
using GameEventSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum POPUP_TYPE
{
    OK,
    OK_CANCEL,
    NO_BUTTON,
}

public class UIPopupMessage : UIPopup
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI text;
    
    [SerializeField] private Button btnOk;
    [SerializeField] private TextMeshProUGUI textOk;

    [SerializeField] private Button btnCenterOk;
    [SerializeField] private TextMeshProUGUI textCenterOk;

    [SerializeField] private Button btnCancel;
    [SerializeField] private TextMeshProUGUI textCancel;

    [SerializeField] private TextMeshProUGUI textTimer;

    public void SetData(string title, string text, POPUP_TYPE type, Action callback1 = null, Action callback2 = null)
    {
        textTimer.SetActive(false);

        this.title.text = title;
        this.text.text = text;

        btnOk.onClick.AddListener(() =>
        {            
            Hide(()=>
            {
                callback1?.Invoke();
            });
        });

        btnCenterOk.onClick.AddListener(() =>
        {            
            Hide(()=>
            {
                callback1?.Invoke();
            });
        });

        btnCancel.onClick.AddListener(() =>
        {            
            Hide(()=>
            {
                callback2?.Invoke();
            });
        });

        btnOk.SetActive(false);
        btnCenterOk.SetActive(false);
        btnCancel.SetActive(false);

        if (type == POPUP_TYPE.OK)
        {
            btnCenterOk.SetActive(true);
        }
        else if( type == POPUP_TYPE.OK_CANCEL)
        {
            btnOk.SetActive(true);
            btnCancel.SetActive(true);
        }
        else if( type == POPUP_TYPE.NO_BUTTON)
        {

        }
    }

    public void SetBtnTextOK(string btnText1)
    {
        textOk.text = btnText1;
        textCenterOk.text = btnText1;
    }

    public void SetBtnTextCancel(string btnText2)
    {
        textCancel.text = btnText2;
    }

    private float remainingTime; // 남은 시간을 저장할 변수
    public void SetTime(float time, Action func)
    {
        if(time == 0)
        {
            return;
        }

        textTimer.SetActive(true);        

        remainingTime = time;        

        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(time);
        sequence.AppendCallback(() =>
        {
            if(gameObject != null)
            {
                Hide();
            }    
            
            func?.Invoke();
        });

        sequence.Play().OnUpdate(() =>
        {            
            // 남은 시간 업데이트
            remainingTime = time - sequence.Elapsed();
            textTimer.text = string.Format(StringManager.Get("UI_AFTER_A_FEW_SECONDS"), remainingTime + 1);
        });
    }      
}
