using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLayer : BaseUI
{
    #region implemented abstract members of BaseUI

    protected override UILayer GetUILayer()
    {
        return UILayer.MainLayer;
    }

    protected override void OnAwake()
    {
        Logger.DebugLog("TestLayer OnAwake");
    }

    protected override void OnStart()
    {

    }

    protected override void OnUpate()
    {

    }

    protected override void OnFixedUpdate()
    {

    }

    protected override void OnLateUpdate()
    {

    }

    #endregion


}
