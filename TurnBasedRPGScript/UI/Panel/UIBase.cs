using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace S7
{
    public enum ShowAniType
    {
        None,
        Left,
        Right,
        Up,
        Down,
        Fade,
    }

    public class UIBase : MonoBehaviour, IDisposable
    {
        CanvasGroup canvasGroup;
        public ShowAniType _showAniType = ShowAniType.None;
        public ShowAniType _hideAniType = ShowAniType.None;

        public bool IsShowing { get => _isShowing; }

        private bool _isShowing;

        protected bool _isInitialize = false;

        private InputAction _backAction;

        protected virtual void Awake()
        {
            DOTween.Init(false, true);

            _backAction = new InputAction(
            "Back",
            InputActionType.Button,
            "<Keyboard>/escape"
            );

            _backAction.Enable();
            _backAction.performed += OnBackSpace;
        }

        protected virtual void OnEnable()
        {
            canvasGroup = GetComponentInChildren<CanvasGroup>();
            if (canvasGroup != null)
            {
                // canvasGroup.alpha = 0;
            }
        }

        protected virtual void OnDestroy()
        {
            transform.DOKill();

            _backAction.performed -= OnBackSpace;
            _backAction.Disable();
        }

        protected virtual void Start()
        {
            UpdateAdBannerHeight();
            FouceScreenUpdate();
        }

        protected virtual void Initialize()
        {
            _isInitialize = true;
        }

        protected virtual void Begin()
        {
            if (_isInitialize) return;
            Initialize();
        }

        public void PrepareInitialize()
        {
            if (_isInitialize) return;
            Begin();
        }

        public virtual void OnClose()
        {
            Hide();
        }

        public virtual void Show()
        {
            gameObject.SetActive(false);
            _isShowing = true;
            Sequence seq = DOTween.Sequence()
                .SetLink(gameObject)
                .SetUpdate(true);

            switch (_showAniType)
            {
                case ShowAniType.Left:
                    {
                        RectTransform rect = GetComponent<RectTransform>();
                        seq.Append(transform.DOLocalMoveX(rect.rect.width, 0.0f));
                    }
                    break;
                case ShowAniType.Right:
                    {
                        RectTransform rect = GetComponent<RectTransform>();
                        seq.Append(transform.DOLocalMoveX(-rect.rect.width, 0.0f));
                    }
                    break;
                case ShowAniType.Up:
                    {
                        RectTransform rect = GetComponent<RectTransform>();
                        seq.Append(transform.DOLocalMoveY(-rect.rect.height, 0.0f));
                    }
                    break;
                case ShowAniType.Down:
                    {
                        RectTransform rect = GetComponent<RectTransform>();
                        seq.Append(transform.DOLocalMoveY(rect.rect.height, 0.0f));
                    }
                    break;
                case ShowAniType.Fade:
                    {
                        if (canvasGroup != null)
                        {
                            canvasGroup.alpha = 0;
                        }
                        break;
                    }
                default:
                    {
                        if (canvasGroup != null)
                        {
                            canvasGroup.alpha = 0;
                        }
                    }
                    break;
            }

            seq.AppendInterval(0.1f);
            seq.AppendCallback(() =>
            {
                SetActiveCanvasGroup(true, 0.2f);
                gameObject.SetActive(true);
            });

            switch (_showAniType)
            {
                case ShowAniType.Left:
                case ShowAniType.Right:
                    {
                        RectTransform rect = GetComponent<RectTransform>();
                        seq.Append(transform.DOLocalMoveX(0, 0.2f).SetEase(Ease.OutCirc));
                    }
                    break;
                case ShowAniType.Up:
                case ShowAniType.Down:
                    {
                        RectTransform rect = GetComponent<RectTransform>();
                        seq.Append(transform.DOLocalMoveY(0, 0.2f).SetEase(Ease.OutCirc));
                    }
                    break;
                case ShowAniType.Fade:
                    {
                        if (canvasGroup != null)
                        {
                            canvasGroup.alpha = 0;
                            seq.Append(canvasGroup.DOFade(1f, 0.2f));
                        }
                        break;
                    }
                default:
                    {
                        if (canvasGroup != null)
                        {
                            seq.Append(canvasGroup.DOFade(1f, 0.2f));
                        }
                    }
                    break;
            }
            seq.Play().SetUpdate(true).OnComplete(() =>
            {
                _isShowing = false;
                Begin();
            });
        }

        public virtual void Hide()
        {
            if (!this) return;

            Sequence seq = DOTween.Sequence()
                .SetLink(gameObject)      // �ٽ�
                .SetUpdate(true);

            switch (_hideAniType)
            {
                case ShowAniType.Left:
                    {
                        RectTransform rect = GetComponent<RectTransform>();
                        seq.Append(transform.DOLocalMoveX(-rect.rect.width, 0.2f));
                    }
                    break;

                case ShowAniType.Right:
                    {
                        RectTransform rect = GetComponent<RectTransform>();
                        seq.Append(transform.DOLocalMoveX(rect.rect.width, 0.2f));
                    }
                    break;

                case ShowAniType.Up:
                    {
                        RectTransform rect = GetComponent<RectTransform>();
                        seq.Append(transform.DOLocalMoveY(rect.rect.height, 0.2f));
                    }
                    break;

                case ShowAniType.Down:
                    {
                        RectTransform rect = GetComponent<RectTransform>();
                        seq.Append(transform.DOLocalMoveY(-rect.rect.height, 0.2f));
                    }
                    break;

                case ShowAniType.Fade:
                    {
                        if (canvasGroup != null)
                        {
                            seq.Append(canvasGroup.DOFade(0f, 0.2f));
                        }
                    }
                    break;
            }

            seq.OnComplete(() =>
            {
                if (!this) return;

                transform.DOKill();
                ResourceManager.Free(gameObject);
            });

            seq.Play();
        }

        public void SetActiveCanvasGroup(bool enable, float fadeTime = 0.0f)
        {
            if (null == canvasGroup)
            {
                return;
            }

            canvasGroup.DOKill();

            if (fadeTime > 0f)
                canvasGroup.DOFade(enable ? 1f : 0f, fadeTime);
            else
                canvasGroup.alpha = enable ? 1f : 0f;
        }

        protected virtual void Update() { }
        protected virtual void FixedUpdate() { }
        public virtual void DisableObejct() { }

        protected virtual void OnBackSpace(InputAction.CallbackContext ctx) { }
        protected virtual void UpdateAdBannerHeight() { }
        protected virtual void FouceScreenUpdate() { }

        public virtual void Dispose() { }
    }

}
