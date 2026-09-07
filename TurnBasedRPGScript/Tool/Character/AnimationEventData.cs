using UnityEngine;

[System.Serializable]
public class AnimationEventData
{
    public float time;
    public AnimationEventType type;
    
    // Effect
    public GameObject prefab;
    public string effectAddressKey;
    public string attachSocket;
    public float duration = 1f;

    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    public string stringParam;
    public float floatParam;
    public int intParam;
    public bool boolParam;
}