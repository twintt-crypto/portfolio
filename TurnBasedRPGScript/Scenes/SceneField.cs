using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace S7
{
    public class SceneField : SceneBase
    {
        private FieldManager _fieldManager;

        public override void Initialize()
        {
            base.Initialize();
        }

        [Inject]
        public void Construct(FieldManager fieldManager)
        {
            _fieldManager = fieldManager;
        }

        public override void OnStart()
        {
            base.OnStart();

            // UIManager.Instance.OpenPanelAsync("UIPanelField").Forget();
            // FieldManager.Instance.SetAreaObjects();
        }           
    }
}

