using UnityEngine.Networking;

namespace Main
{
    public class MessageInt : MessageBase
    {
        public int ID;

        public override void Deserialize(NetworkReader reader)
        {
            ID = reader.ReadInt16();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(ID);
        }
    }

}
