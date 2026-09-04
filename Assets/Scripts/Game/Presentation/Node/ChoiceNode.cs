using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace S7
{
    public class ChoiceOption
    {
        public string portName;
        public RuntimeNode nextNode;
    }

    public class ChoiceNode : BasePresentationNode
    {
        public List<ChoiceOption> options = new();

        public ChoiceNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
        }

        public override async UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (ctx == null)
            {
                Debug.LogWarning("ChoiceNode: ctx is null");
                return;
            }

            if (options == null || options.Count == 0)
            {
                Debug.LogWarning($"ChoiceNode: options is null or empty. title={_data.title}");
                return;
            }

            string choiceKey = _data.param1;
            if (string.IsNullOrWhiteSpace(choiceKey))
            {
                Debug.LogWarning($"ChoiceNode: param1(choiceKey) is empty. title={_data.title}");
                return;
            }

            int selectedIndex;

            try
            {
                selectedIndex = await ctx.ShowChoiceAsync(choiceKey);
            }
            catch (System.OperationCanceledException)
            {
                throw;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"ChoiceNode: ShowChoiceAsync failed. title={_data.title}, key={choiceKey}, error={ex}");
                return;
            }

            token.ThrowIfCancellationRequested();

            if (selectedIndex < 0 || selectedIndex >= options.Count)
            {
                Debug.LogWarning(
                    $"ChoiceNode: invalid selectedIndex={selectedIndex}, optionCount={options.Count}, title={_data.title}, key={choiceKey}");
                return;
            }

            ChoiceOption option = options[selectedIndex];
            if (option == null)
            {
                Debug.LogWarning(
                    $"ChoiceNode: selected option is null. selectedIndex={selectedIndex}, title={_data.title}");
                return;
            }

            if (option.nextNode == null)
            {
                Debug.LogWarning(
                    $"ChoiceNode: selected next node is null. selectedIndex={selectedIndex}, portName={option.portName}, title={_data.title}");
                return;
            }

            await GraphExecutor.Execute(option.nextNode, ctx, token);
        }
    }

}
