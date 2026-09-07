using Cysharp.Threading.Tasks;
using GameEventSystem;
using Gpm.Ui;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;


namespace S7
{
    public class UIPanelBattleBase : UIBase
    {
        protected BattleManager _battleManager;

        public virtual async UniTask Initialize(BattleManager battleManager)
        {
            _battleManager = battleManager;
            await UniTask.CompletedTask;
        }
    }

}


