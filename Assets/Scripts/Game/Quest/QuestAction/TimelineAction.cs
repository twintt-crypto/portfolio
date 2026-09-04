using Cysharp.Threading.Tasks;
using DialogueSystem;
using S7;
using UnityEngine.Playables;

public class TimelineAction : IQuestAction
{
    public PlayableAsset timeline;

    public async UniTask Execute()
    {
        await UniTask.CompletedTask;
        //wait StoryManager.Instance.Play(timeline);
    }
}