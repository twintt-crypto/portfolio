using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using Cysharp.Threading.Tasks;
using S7;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TextCore.Text;

public class SkillPreviewRunner
{
    private PreviewUnitController _character;
    private PreviewUnitController _monster;

    private CancellationTokenSource _cts;

    public async UniTask PreviewCharacter(T_UnitData data, Transform parent)
    {
        UnitView unitView = parent.GetComponentInChildren<UnitView>();
        if(unitView != null && unitView.Model != null)
        {
            Addressables.ReleaseInstance(unitView.Model);
        }        

        _character = new PreviewUnitController();
        await _character.Initialize(data, parent);

        _cts = new CancellationTokenSource();        

    }

    public async UniTask PreviewMonster(T_UnitData data, Transform parent)
    {

        UnitView unitView = parent.GetComponentInChildren<UnitView>();
        if (unitView != null)
        {
            Addressables.ReleaseInstance(unitView.Model);
        }

        _monster = new PreviewUnitController();
        await _monster.Initialize(data, parent);

        _cts = new CancellationTokenSource();
    }


    public bool IsPlay = false;
    public async void Play(PresentationGraphAsset graph)
    {       
        if (graph == null)
        {
            Debug.LogWarning("Graph ����");
            return;
        }

        if( _character.isInitialize == false)
        {
            return;
        }

        if(IsPlay == true)
        {
            return;
        }

        _cts = new CancellationTokenSource();                

        var ctx = SkillPreviewContextBuilder.Build(_character, _monster);        

        bool hasTimelineNode = false;
        if (graph.nodes != null)
        {
            for (int i = 0; i < graph.nodes.Count; i++)
            {
                if (graph.nodes[i].nodeType == PresentationNodeType.PlayTimeline)
                {
                    hasTimelineNode = true;
                    break;
                }
            }
        }

        if (hasTimelineNode)
        {
            var timelineProvider = new TimelineAddressableProvider();

            ctx.getTimelineAsync = timelineProvider.GetAsync;
            ctx.releaseTimeline = timelineProvider.ReleaseTimelineImpl;
        }

        PresentationRuntimeGraph runtimeGraph = PresentationRuntimeGraphBuilder.Build(graph);

        if (runtimeGraph == null)
        {
            Debug.LogWarning("PresentationCore: runtimeGraph is null");
            return;
        }

        if (runtimeGraph.startNode == null)
        {
            Debug.LogWarning("PresentationCore: startNode is null");
            return;
        }

        try
        {
            IsPlay = true;
            await GraphExecutor.Execute(runtimeGraph.startNode, ctx, _cts.Token);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }

        await UniTask.Delay(500);
        _character.View.transform.localPosition = Vector3.zero;
        _character.View.transform.localRotation = Quaternion.identity;

        IsPlay = false;
    }

    public void Stop()
    {
        _cts?.Cancel();
        _cts = null;

        //if (_character != null)
           // Addressables.ReleaseInstance(_character);

        //if (_monster != null)
            //Addressables.ReleaseInstance(_monster);
    }

    public void Destroy()
    {
        _cts?.Cancel();
        _cts = null;

        if (_character != null)
        {

            _character.Release();
            _character = null;
        }            

        if (_monster != null)
        {
            _monster.Release();
            _monster = null;
        }         
    }    
}