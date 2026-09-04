using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum PopupAniType
{
    Default,
    Left,
    Right,
    LocalZero,    
    Up,
    Down,
    Instantly,
    Return,
    NotUse,
}

public class UIPopupBase : MonoBehaviour, IPointerDownHandler
{
    public PopupAniType _showType = PopupAniType.Default;
    public AnimationCurve _show;

    public PopupAniType _hideType = PopupAniType.Default;
    public AnimationCurve _hide;

    private CanvasGroup _canvasGroup;
   

    private string _popupName;
    [SerializeField] protected RectTransform _content;    

    private Vector3 _startLocalPos;
    private Quaternion _startQuaternion;

    public string PopupName { get => _popupName; }
    public bool _bgClickHide = true;

    protected bool _isHide = false;

    protected virtual void Awake()
    {
        _canvasGroup = gameObject.GetComponentInChildren<CanvasGroup>();
    }        

    public virtual void Show(Action startAction = null , Action complate = null)
    {
        if (_content == null)
        {
            Debug.LogError("root is null");
            return;
        }

        if (null != _canvasGroup)
        {
            _canvasGroup.alpha = 0;
        }
        
        _startLocalPos = _content.transform.localPosition;
        _startQuaternion = _content.transform.rotation;
        
        Sequence seq = DOTween.Sequence();

        //초기 셋팅
        switch (_showType)
        {
            case PopupAniType.Default:
                {
                    seq.Append(_content.DOScale(0.0f, 0.0f));
                }
                break;
            case PopupAniType.Left:
                {
                    if (_content != null)
                    {
                        RectTransform rect = GetComponent<RectTransform>();
                        seq.Append(_content.DOLocalMoveX(rect.rect.width, 0.0f));
                    }
                }
                break;
            case PopupAniType.Right:
                {
                    if (_content != null)
                    {
                        RectTransform rect = GetComponent<RectTransform>();
                        seq.Append(_content.DOLocalMoveX(-rect.rect.width, 0.0f));
                    }
                }
                break;
            case PopupAniType.Up:
                {
                    if (_content != null)
                    {
                        RectTransform rect = GetComponent<RectTransform>();
                        seq.Append(_content.DOLocalMoveY(-rect.rect.height, 0.0f));
                    }
                }
                break;
            case PopupAniType.Down:
                {
                    if (_content != null)
                    {
                        RectTransform rect = GetComponent<RectTransform>();
                        seq.Append(_content.DOLocalMoveY(rect.rect.height, 0.0f));
                    }
                }
                break;
            case PopupAniType.NotUse:
                {
                    gameObject.SetActive(true);                    
                }
                break;
        }

        //seq.AppendInterval(0.2f);
        seq.AppendCallback(() =>
        {
            if (null != _canvasGroup)
            {
                _canvasGroup.alpha = 1;
            }            
        });

        //동작
        switch (_showType)
        {
            case PopupAniType.Default:
                {                    
                    seq.Append(_content.DOScale(1.0f, 0.3f).SetEase(_show));
                }
                break;            
            case PopupAniType.LocalZero:
                {
                    if (_content != null)
                    {
                        seq.Insert(0, _content.DORotate(Vector3.zero, 0.3f).SetEase(Ease.OutQuint));
                    }
                }
                break;
            case PopupAniType.Up:
            case PopupAniType.Down:
                {
                    if (_content != null)
                    {
                        RectTransform rect = GetComponent<RectTransform>();
                        seq.Append(_content.DOLocalMoveY(0, 0.3f).SetEase(Ease.OutQuint));
                    }
                }
                break;
            case PopupAniType.Right:
            case PopupAniType.Left:

                {
                    if (_content != null)
                    {
                        RectTransform rect = GetComponent<RectTransform>();
                        seq.Append(_content.DOLocalMoveX(0, 0.3f).SetEase(Ease.OutQuint));
                    }
                }
                break;            
        }
        
        seq.AppendCallback(() =>
        {
            complate?.Invoke();
            OnBegin();
        });
        seq.Play().SetUpdate(true);

    }

    public virtual void Hide(Action complate = null)
    {
        if (_content == null)
        {
            Debug.LogError("Hide root is null");
            return;
        }

        _isHide = true;        

        Debug.Log($"Hide Popup : {gameObject.name}");

        Sequence seq = DOTween.Sequence();        
        switch (_hideType)
        {
            case PopupAniType.Default:
                {
                    seq.Append(_content.DOScale(0.0f, 0.1f).SetEase(_hide));
                }
                break;
            case PopupAniType.Left:
                {
                    RectTransform rect = GetComponent<RectTransform>();
                    seq.Append(_content.DOLocalMoveX(-rect.rect.width, 0.1f).SetEase(Ease.OutCirc));
                }
                break;
            case PopupAniType.Right:
                {
                    RectTransform rect = GetComponent<RectTransform>();
                    seq.Append(_content.DOLocalMoveX(rect.rect.width, 0.1f).SetEase(Ease.OutCirc));
                }
                break;
            case PopupAniType.Up:
                {
                    RectTransform rect = GetComponent<RectTransform>();
                    seq.Append(_content.DOLocalMoveY(rect.rect.height, 0.1f).SetEase(Ease.OutCirc));
                }
                break;

            case PopupAniType.Down:
                {
                    RectTransform rect = GetComponent<RectTransform>();
                    seq.Append(_content.DOLocalMoveY(-rect.rect.height, 0.1f).SetEase(Ease.OutCirc));
                }
                break;
            case PopupAniType.Return:
                {
                    seq.Append(_content.DOMove(_startLocalPos, 0.1f).SetEase(_hide));
                    seq.Insert(0, _content.DORotateQuaternion(_startQuaternion, 0.1f).SetEase(Ease.OutCirc));
                }
                break;                
        }
        
        seq.AppendInterval(0.1f);
        seq.OnComplete(() =>
        {
            complate?.Invoke();
            if (null != _canvasGroup)
            {
                _canvasGroup.alpha = 0;
            }

            RemovePopupQueueData(PopupName);
            ResourceManager.Free(gameObject);
        });        
        seq.Play();
    }
    protected virtual void RemovePopupQueueData(string popupName)
    {
    }

    public void SetPopupName(string name)
    {
        gameObject.name = name;
        _popupName = name;
    }

    protected virtual void onPointDown(PointerEventData eventData)
    {
      
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointDown(eventData);
    }   

    public void SetShowType(PopupAniType type)
    {
        _showType = type;
    }

    public void SetHideType(PopupAniType type)
    {
        _hideType = type;
    }

    public virtual void OnBegin()
    {

    }


    public void OnApplicationQuit()
    {

    }

    protected virtual void OnDestroy()
    {
        _content?.DOKill();
    }
}
