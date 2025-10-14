using System;
using UnityEngine;

public class PoolableObject : MonoBehaviour
{
    private Action<PoolableObject> _returnToPoolAction;

    public void SetReturnAction(Action<PoolableObject> returnAction)
    {
        _returnToPoolAction = returnAction;
    }

    public void ReturnToPool()
    {
        _returnToPoolAction?.Invoke(this);
    }
}