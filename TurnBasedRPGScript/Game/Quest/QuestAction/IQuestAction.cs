using Cysharp.Threading.Tasks;
using UnityEngine;

namespace S7
{
    public interface IQuestAction
    {
        UniTask Execute();
    }
}