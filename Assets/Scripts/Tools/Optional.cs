using System;
using UnityEngine;

/// <summary>
/// Optional By Jacob "Aarthificial"
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable]
public struct Optional<T>
{
    [SerializeField] private bool enabled;
    [SerializeField] private T value;

    public bool Enabled => enabled;
    public T Value => value;

    public Optional(T initialValue)
    {
        enabled = true;
        value = initialValue;
    }
}