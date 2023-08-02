using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Main
{
    public class MessageGameobject : MessageBase
    {
        public GameObject Crystall;

        public override void Deserialize(NetworkReader reader)
        {
            Crystall = reader.ReadGameObject();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(Crystall);
        }
    }
}