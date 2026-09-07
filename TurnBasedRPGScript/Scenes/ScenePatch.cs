using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace S7
{
    public class ScenePatch : SceneBase
    {
        public override void Initialize()
        {
            base.Initialize();            
        }

        protected override async UniTask PreLoadAsync()
        {
            await OpenPanelPatch();
        }

        private async UniTask OpenPanelPatch()
        {
            UIPanelPatch panel = await UIManager.Instance.OpenPanelAsync("UIPanelPatch") as UIPanelPatch;
            if (panel == null)
            {
                return;
            }

            panel.Patch().Forget();
        }       
    }
}
