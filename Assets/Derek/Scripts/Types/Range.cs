using System;
using UnityEngine;

[Serializable]
public struct Range<T>
{
    [SerializeField] private T _min;
    public readonly T Min => _min;

    [SerializeField] private T _max;
    public readonly T Max => _max;
}
