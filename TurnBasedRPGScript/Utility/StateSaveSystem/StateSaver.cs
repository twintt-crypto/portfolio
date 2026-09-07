using UnityEngine;
using UnityEngine.SceneManagement;

namespace S7
{
    public static class StateSaver
    {
        public static void Capture(StateSnapshot snapshot, params Scene[] scenes)
        {
            foreach (Scene scene in scenes)
            {
                foreach (GameObject go in scene.GetRootGameObjects())
                {
                    // 꺼져있는 요소도 탐색할지, 현재는 꺼져있으면 저장 필요없음
                    // foreach (IStateSaveable saveable in go.GetComponentsInChildren<IStateSaveable>(true))
                    foreach (IStateSaveable saveable in go.GetComponentsInChildren<IStateSaveable>())
                    {
                        saveable.CaptureState(snapshot);
                    }
                }
            }
        }

        public static void Restore(StateSnapshot snapshot, params Scene[] scenes)
        {
            foreach (Scene scene in scenes)
            foreach (GameObject go in scene.GetRootGameObjects())
            foreach (IStateSaveable saveable in go.GetComponentsInChildren<IStateSaveable>(true))
                saveable.RestoreState(snapshot);
        }
    }
}
