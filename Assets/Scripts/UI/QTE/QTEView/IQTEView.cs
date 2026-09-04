using Cysharp.Threading.Tasks;
using Game.QTE;

namespace UI.QTE
{
    public interface IQTEView
    {
        void Setup(QTEConfig config);
        UniTask ShowResultAsync(QTE_RESULT result);
    }
}
