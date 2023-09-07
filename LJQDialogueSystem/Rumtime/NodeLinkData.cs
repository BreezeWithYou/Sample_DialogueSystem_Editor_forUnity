using System;
using UnityEngine;

/// <summary>
/// 该类存储了结点之间的连线信息
/// </summary>
[Serializable]
public class NodeLinkData
{
    public string BaseNodeGUID;
    public string PortName;
    public string TargetNodeGUID;
}