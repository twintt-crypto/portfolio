using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class CommonUtil
{
    public static void DelayedCall(float delay, System.Action callback)
    {
        DOVirtual.DelayedCall(delay, () =>
        {
            callback.Invoke();
        });
    }

    public static void NextFrameCall(System.Action callback)
    {
        DOVirtual.DelayedCall(0.0f, () =>
        {
            callback.Invoke();
        });
    }

    public static void UpdateCurrency(TextMeshProUGUI text, ulong end, float duration = 0.5f)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(DOTween.To(() => 0, x => text.text = x.ToString(), (int)end, duration)
            .SetEase(Ease.Linear));
        seq.Play();
    }

    

    public static string ToJson<T>(List<T> list, bool prettyPrint = false)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = list;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    public static List<T> FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    [System.Serializable]


    private class Wrapper<T>
    {
        public List<T> Items;
    }

    public static void SetWorldToCanvas(Vector3 worldPosition, Camera mainCamera, Camera uiCamera, Transform trThis, RectTransform rectCanvas = null)
    {
        if (null == mainCamera)
        {
            return;
        }


        if (null == uiCamera)
        {
            return;
        }

        if (null == trThis)
        {
            return;
        }

        if (rectCanvas == null)
        {
            var goCanvas = GetComponentInRecursiveParent<Canvas>(trThis.gameObject).gameObject;
            goCanvas.TryGetComponent(out rectCanvas);
        }

        var screenPoint = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPosition);

        Vector2 uiLocalPosition;
        if (true == RectTransformUtility.ScreenPointToLocalPointInRectangle(rectCanvas, screenPoint, uiCamera,
            out uiLocalPosition))
        {
            trThis.localPosition = uiLocalPosition;
        }
    }

    public static Vector2 WorldToCanvasPosition(RectTransform canvas, Camera camera, Vector3 position)
    {
        //Vector position (percentage from 0 to 1) considering camera size.
        //For example (0,0) is lower left, middle is (0.5,0.5)
        Vector2 temp = camera.WorldToViewportPoint(position);

        //Calculate position considering our percentage, using our canvas size
        //So if canvas size is (1100,500), and percentage is (0.5,0.5), _current value will be (550,250)
        temp.x *= canvas.sizeDelta.x;
        temp.y *= canvas.sizeDelta.y;

        //The result is ready, but, this result is correct if canvas recttransform pivot is 0,0 - left lower corner.
        //But in reality its middle (0.5,0.5) by default, so we remove the amount considering cavnas rectransform pivot.
        //We could multiply with constant 0.5, but we will actually read the value, so if custom rect transform is passed(with custom pivot) , 
        //returned value will still be correct.

        temp.x -= canvas.sizeDelta.x * canvas.pivot.x;
        temp.y -= canvas.sizeDelta.y * canvas.pivot.y;

        return temp;
    }

    public static T GetComponentInRecursiveParent<T>(GameObject goParent) where T : Component
    {
        while (null != goParent)
        {
            if (true == goParent.TryGetComponent(out T comp))
            {
                return comp;
            }
            else
            {
                goParent = goParent.transform?.parent?.gameObject;
            }
        }

        return null;
    }

    public static float GetUpdateEffectTime(long start, long end)
    {
        // 1만 = 0.6초 기준 min : 0.25f , max : 1f
        var gap = Mathf.Abs(end - start);

        return Mathf.Clamp((gap / 10000) * 0.6f, 0.25f, 1f);
    }

    public static string txtPrice(double currency, string strCurrency = "")
    {
        if (currency == 0)
        {
            return "0";
        }

        return string.Format("{0:#,###.##}{1}", currency, strCurrency);
    }

    public static string ReplaceTime(string text)
    {
        return text.Replace("#now_time#", $"<b><color=#1E2A78>{DateTime.Now.ToLocalTime()}</color></b>");
    }

    public static int GetAppVersion(string version)
    {
        string[] appVersion = version.Split('.');
        return (Convert.ToInt32(appVersion[0]) * 1000000) + (Convert.ToInt32(appVersion[1]) * 1000) + (Convert.ToInt32(appVersion[2])); ;
    }

    public static Vector3 GetAttackPosition(Transform attacker, Collider targetCollider, float offset = 0.1f)
    {
        // 공격자 위치에서 타겟 방향 벡터
        Vector3 dir = (targetCollider.transform.position - attacker.position).normalized;

        // 타겟 콜라이더 표면 중 attacker와 가장 가까운 점
        Vector3 closestPoint = targetCollider.ClosestPoint(attacker.position);

        // 살짝 떨어뜨려서 겹침 방지
        Vector3 finelPos = closestPoint - dir * offset;
        finelPos.y = 0;
        return finelPos;
    }
}
