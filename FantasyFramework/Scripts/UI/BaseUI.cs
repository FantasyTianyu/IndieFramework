using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{

    protected abstract UILayer GetUILayer();

    #region 代理回调函数

    protected abstract void OnAwake();

    protected abstract void OnStart();

    protected abstract void OnUpate();

    protected abstract void OnFixedUpdate();

    protected abstract void OnLateUpdate();

    #endregion

    protected virtual void SetLayer()
    {
        UIManager.Instance.ChangeLayer(transform, GetUILayer());
    }

    private void Awake()
    {
        SetLayer();
        OnAwake();
    }

    private void Start()
    {
        OnStart();
    }

    private void Update()
    {
        OnUpate();
    }

    private void FixedUpdate()
    {
        OnFixedUpdate();
    }

    private void LateUpdate()
    {
        OnLateUpdate();
    }


}
