using Gpm.Ui;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace S7
{
    public class UIPanelLobby : UIBase
    {
        [SerializeField] InfiniteScroll _infiniteScroll;

        protected override void Start()
        {
            for (int i = 0; i < 10; i++)
            {

                TempData data = new TempData(); ;
                data.index = i;
                _infiniteScroll.InsertData(data);
            }
        }

        protected override void OnDestroy()
        {
        }
    }
}

