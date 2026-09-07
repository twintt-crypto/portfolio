using Cysharp.Threading.Tasks;
using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

 namespace S7
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private Transform _uiTr;

        [SerializeField] private Camera _camera;
        [SerializeField] private Canvas _canvas;

        [SerializeField] private CanvasGroup _fadeCanvasGroup;

        private Dictionary<string, UIBase> uiList = new Dictionary<string, UIBase>();

        public Camera Camera { get => _camera; }
        public Canvas Canvas { get => _canvas; }

        public bool isUsableBackSpace = false;

        // Start is called before the first frame update
        void Start()
        {

        }

        public void SetAdHeight(float height)
        {

        }

        public async UniTask<UIBase> OpenPanelAsync(string uiName, bool showImmediately = true)
        {
            if (uiList.TryGetValue(uiName, out var exist))
            {
                ClosePanel(exist);
            }

            Transform tr = _uiTr;

            var go = await ResourceManager.NewAsync(uiName, tr, usePooling: false);

            go.name = uiName;

            var uiBase = go.GetComponent<UIBase>();
            if (uiBase == null)
            {
                Destroy(go);
                return null;
            }

            if (showImmediately)
            {
                uiBase.Show();
            }
            else
            {
                uiBase.PrepareInitialize();
                go.SetActive(false);
            }
            uiList.Add(uiName, uiBase);

            return uiBase;
        }

        public async UniTask<UIBase> OpenPanelAsync(string uiName, Transform parent, bool showImmediately = true)
        {
            if (uiList.TryGetValue(uiName, out var exist))
            {
                ClosePanel(exist);
            }

            var go = await ResourceManager.NewAsync(uiName, parent, false);
            go.name = uiName;
            var uiBase = go.GetComponent<UIBase>();
            if (uiBase == null)
            {
                Destroy(go);
                return null;
            }

            if (showImmediately)
            {
                uiBase.Show();
            }
            else
            {
                uiBase.PrepareInitialize();
                go.SetActive(false);
            }
            uiList.Add(uiName, uiBase);

            return uiBase;
        }

        public void ShowPanel(string uiName)
        {
            if (uiList.TryGetValue(uiName, out UIBase ui))
            {
                ui.gameObject.SetActive(true);
                ui.Show();
            }
        }

        public void ClosePanel(string uiName)
        {
            if (uiList.ContainsKey(uiName))
            {
                uiList[uiName].OnClose();
                uiList.Remove(uiName);
            }
        }

        public void ClosePanel(UIBase ui)
        {
            if (uiList.ContainsKey(ui.name))
            {
                uiList[ui.name].OnClose();
                uiList.Remove(ui.name);
            }
        }

        public void DestroyPanel(string uiName)
        {
            if (uiList.ContainsKey(uiName))
            {
                uiList[name].Dispose();
                ResourceManager.Free(uiList[uiName].gameObject);
                uiList.Remove(uiName);
            }
        }

        public void DestroyPanel(UIBase ui)
        {
            if (uiList.ContainsKey(ui.name))
            {
                uiList[ui.name].Dispose();
                uiList.Remove(ui.name);
                ResourceManager.Free(ui.gameObject);
            }
        }

        public void CloseAll()
        {
            foreach (var iter in uiList)
            {
                ResourceManager.Free(iter.Value.gameObject);
            }

            uiList.Clear();
        }

        public UIBase GetPanel(string name)
        {
            if (uiList.TryGetValue(name, out UIBase ui) == false)
            {
                return null;
            }

            return ui;
        }

        public bool IsOpen(string name)
        {
            if (uiList.TryGetValue(name, out UIBase ui) == false)
            {
                return false;
            }

            return true;
        }

        public async UniTask FadeOutAsync()
        {
            if(_fadeCanvasGroup == null)
            {
                return;
            }

            _fadeCanvasGroup.alpha = 0;
            float duration = 0.5f;
            await _fadeCanvasGroup.DOFade(1, duration).SetUpdate(true).ToUniTask();
        }

        public async UniTask FadeInAsync()
        {
            if (_fadeCanvasGroup == null)
            {
                return;
            }

            _fadeCanvasGroup.alpha = 1;
            float duration = 0.5f;
            await _fadeCanvasGroup.DOFade(0, duration).SetUpdate(true).ToUniTask();
        }

        List<UnitUI> unitUiList = new List<UnitUI>();

        public void RegisterUnitUi(UnitUI ui)
        {
            unitUiList.Add(ui);
        }

        public void UnRegisterUnitUi(UnitUI ui)
        {
            unitUiList.Remove(ui);
        }

        void LateUpdate()
        {
            Camera cam = GetCurrentCamera();
            if (cam == null)
                return;

            for (int i = 0; i < unitUiList.Count; i++)
            {
                var unitUi = unitUiList[i];

                if (unitUi.TopPoint == null || unitUi.TopPoint == null)
                    continue;

                Vector3 pos = cam.WorldToScreenPoint(unitUi.TopPoint.position + unitUi.offset);

                if (pos.z < 0)
                {
                    unitUi.Nameplate.gameObject.SetActive(false);
                    continue;
                }

                unitUi.Nameplate.gameObject.SetActive(true);
                unitUi.Nameplate.transform.position = pos;
            }
        }

        Camera GetCurrentCamera()
        {
            Camera[] cams = Camera.allCameras;

            Camera result = null;
            float maxDepth = float.MinValue;

            int uiLayer = LayerMask.NameToLayer("UI");

            foreach (var cam in cams)
            {
                if (!cam.enabled)
                    continue;

                if (cam.cullingMask == (1 << uiLayer))
                    continue;

                if (cam.depth > maxDepth)
                {
                    maxDepth = cam.depth;
                    result = cam;
                }
            }

            return result;
        }

    }
}
