using Cysharp.Threading.Tasks;
using DialogueSystem;
using S7;
using UnityEngine.Playables;

public class RewardAction : IQuestAction
{
    public int itemId;
    public int count;

    public async UniTask Execute()
    {
        // PlayerDataManager.Instance.AddItem(itemId, count);
        await UniTask.CompletedTask;        
    }
}