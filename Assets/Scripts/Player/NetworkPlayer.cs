using UnityEngine;
using UnityEngine.Networking;
#pragma warning disable 618
using static UnityEngine.Networking.NetworkServer;
#pragma warning restore 618

namespace Players
{
#pragma warning disable 618
    public class NetworkPlayer : NetworkBehaviour
#pragma warning restore 618
    {
        [SerializeField] private GameObject playerPrefab;
        private GameObject playerCharacter;

        private void Start()
        {
            SpawnCharacter();
        }

        private void SpawnCharacter()
        {
            if (!isServer)
            {
                return;
            }

            playerCharacter = Instantiate(playerPrefab, transform.position, transform.rotation);
            

            SpawnWithClientAuthority(playerCharacter, connectionToClient);
        }
    }
}
