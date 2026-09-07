using UnityEngine;
using S7;
using Cysharp.Threading.Tasks;

namespace S7
{
    public class SceneRoot : SceneBase
    {
        void Start()
        {
            Global.Instance.Initialize();
            GameSceneManager.Instance.LoadScene(SceneType.ScenePatch).Forget();
        }
    }
}

