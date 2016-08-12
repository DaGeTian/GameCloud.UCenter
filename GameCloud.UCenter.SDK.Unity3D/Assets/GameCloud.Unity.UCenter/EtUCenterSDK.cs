// Copyright(c) Cragon.All rights reserved.

namespace GameCloud.UCenter
{
    using System;
    using System.Collections.Generic;
    using GameCloud.Unity.Common;

    public class EtUCenterSDK : EntityDef
    {
        //-------------------------------------------------------------------------
        public override void declareAllComponent(byte node_type)
        {
            declareComponent<DefUCenterSDK>();
        }
    }
}
