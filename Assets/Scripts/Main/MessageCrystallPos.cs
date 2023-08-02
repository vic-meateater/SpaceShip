using UnityEngine;
using UnityEngine.Networking;

namespace Main
{
    public class MessageCrystallPos : MessageBase
    {
        public Vector3 CrystallPos;

        public override void Deserialize(NetworkReader reader)
        {
            CrystallPos = reader.ReadVector3();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(CrystallPos);
        }
    }
}
