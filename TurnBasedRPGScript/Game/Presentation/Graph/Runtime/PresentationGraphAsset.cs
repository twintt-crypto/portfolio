using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PresentationGraph", menuName = "Tools/Presentation Graph")]
public class PresentationGraphAsset : ScriptableObject
{
    public List<PresentationNodeData> nodes = new();
    public List<PresentationEdgeData> edges = new();
}