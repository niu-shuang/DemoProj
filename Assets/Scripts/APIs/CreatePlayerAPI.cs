using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DemoProj
{
    public class CreatePlayerAPI : APIBase<CreatePlayerDTO>
    {
        protected override string apiName => "create_player.php";

        public CreatePlayerAPI()
        {
            AddPostParam("deviceID", SystemInfo.deviceUniqueIdentifier);
        }
    }

    [Serializable]
    public class CreatePlayerDTO
    {
        public string uid;
    }
}
