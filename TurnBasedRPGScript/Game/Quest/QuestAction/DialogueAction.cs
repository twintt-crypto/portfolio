using Cysharp.Threading.Tasks;
using DialogueSystem;
using S7;

public class DialogueAction : IQuestAction
{
    public int dialogueId;

    public async UniTask Execute()
    {
        await UniTask.CompletedTask;
        //await DialogueManager.Instance.Play(dialogueId);
    }
}