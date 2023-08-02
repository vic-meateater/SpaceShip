using UnityEngine.Networking;

namespace Main
{
    public class MessageLogin : MessageBase
    {
        public string login;

        public override void Deserialize(NetworkReader reader)
        {
            login = reader.ReadString();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(login);
        }
    }

}
