using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupManager : Singleton<UIPopupManager>
{
    [SerializeField] private Transform _popupTr;

    private List<UIPopupQueueData> popupQueue = new List<UIPopupQueueData>();

    private UIPopupQueueData currentVisibleQueuePopup;

    public void HidePopup(string popupName = "", bool instantly = false)
    {
        if (0 == popupQueue.Count)
        {
            return;
        }

        if (popupName.IsNullOrEmpty() == true)
        {
            UIPopupQueueData data = popupQueue[popupQueue.Count - 1];

            if (instantly == true)
            {
                data.popup.SetHideType(PopupAniType.Instantly);
            }

            for (int i = 0; i < popupQueue.Count; i++)
            {
                UIPopupQueueData popupQueueData = popupQueue[i];
                if (data == null)
                {
                    continue;
                }

                if (data.popupName == popupName)
                {
                    if (popupQueue.Count > i + 1)
                    {
                        UIPopupQueueData nextPopup = popupQueue[i + 1];
                        if (nextPopup.popup.gameObject.activeSelf == false)
                        {
                            nextPopup.popup.Show();
                            break;
                        }
                    }
                }
            }
            
            data.popup.Hide(() =>
            {
                UIPopupQueueData lastPopup = GetLastPopupData();
                if (lastPopup == null)
                {
                    currentVisibleQueuePopup = null;
                }
                else
                {
                    lastPopup.Show();
                    currentVisibleQueuePopup = lastPopup;
                }
            });
        }
        else
        {
            for (int i = 0; i < popupQueue.Count; i++)
            {
                UIPopupQueueData data = popupQueue[i];
                if (data == null)
                {
                    continue;
                }

                if (data.popupName == popupName)
                {
                    if (instantly == true)
                    {
                        data.popup.SetHideType(PopupAniType.Instantly);
                    }

                    data.popup.Hide();
                    return;
                }
            }
        }
    }

    public void RemovePopupQueueData(string popupName)
    {
        for (int i = 0; i < popupQueue.Count; i++)
        {
            UIPopupQueueData data = popupQueue[i];
            if (data == null)
            {
                continue;
            }

            if (data.popup.name == popupName)
            {
                if (currentVisibleQueuePopup != null && currentVisibleQueuePopup.popupName == popupName)
                {
                    if (i > 0)
                    {
                        currentVisibleQueuePopup = popupQueue[i - 1];
                    }
                    else
                    {
                        currentVisibleQueuePopup = null;
                    }
                }

                if (popupQueue.Count > i + 1)
                {
                    UIPopupQueueData nextPopup = popupQueue[i + 1];
                    DOVirtual.DelayedCall(0.1f, () =>
                    {
                        if (nextPopup.popup.gameObject.activeSelf == false)
                        {
                            nextPopup.popup.Show();
                            currentVisibleQueuePopup = nextPopup;
                        }
                    });
                }

                popupQueue.RemoveAt(i);
                break;
            }
        }       
    }

    public UIPopupQueueData GetCurrentVisibleQueuePopup()
    {
        return currentVisibleQueuePopup;
    }

    public UIPopupQueueData GetLastPopupData()
    {
        if (popupQueue.Count == 0)
        {
            return null;
        }

        return popupQueue[popupQueue.Count - 1];
    }

    public void HidePopupAll()
    {
        Debug.Log("HideAll");
        foreach (var iter in popupQueue)
        {
            Destroy(iter.popup.gameObject);
        }

        popupQueue.Clear();
        currentVisibleQueuePopup = null;
    }

    public UIPopup GetPopup(string popupName)
    {
        foreach (var popup in popupQueue)
        {
            if (popup.popupName == popupName)
            {
                return popup.popup;
            }
        }
        return null;
    }    

    public async UniTask<UIPopup> ShowPopupAsync(string prefabName, string popupName = "", bool immediately = true)
    {
        if (popupName.IsNullOrEmpty() == true)
        {
            popupName = prefabName;
        }

        UIPopup popup = GetPopup(popupName);
        if(popup != null)
        {
            RemovePopup(popupName);
            ResourceManager.Free(popup.gameObject);            
        }        

        var go = await ResourceManager.NewAsync(prefabName, transform, false);
        popup = go.GetComponent<UIPopup>();
        if (null == popup)
        {
            ResourceManager.Free(go);
            return null;
        }

        popup.SetPopupName(popupName);

        UIPopupQueueData data = new UIPopupQueueData
        {
            popup = popup,
            popupName = popupName,
        };

        if (immediately == true)
        {
            popup.gameObject.SetActive(true);
            currentVisibleQueuePopup = data;
            currentVisibleQueuePopup.popup.Show();
        }
        else
        {
            if (currentVisibleQueuePopup == null)
            {
                popup.gameObject.SetActive(true);
                currentVisibleQueuePopup = data;
                currentVisibleQueuePopup.popup.Show();
            }
            else
            {
                popup.gameObject.SetActive(false);
            }
        }

        AddPopup(data);
        return popup;
    }

    public void AddPopup(UIPopupQueueData data)
    {
        popupQueue.Add(data);
    }

    private void RemovePopup(string popupName)
    {
        for (var i = 0; i < popupQueue.Count; i++)
        {
            if (popupQueue[i].popupName == popupName)
            {
                popupQueue.RemoveAt(i);
                return;
            }
        }
    }

    public bool IsShowPopup(string popupName = "")
    {
        if( popupName.Equals("") == true)
        {
            return popupQueue.Count > 0 ? true : false;
        }
        else
        {
            for (var i = 0; i < popupQueue.Count; i++)
            {
                if (popupQueue[i].popupName == popupName && popupQueue[i].popup != null)
                {
                    return true;
                }
            }
        }        

        return false;
    }   
}
