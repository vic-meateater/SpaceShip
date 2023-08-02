using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 618
public class SpawnPoint : NetworkStartPosition
#pragma warning restore 618
{
    [SerializeField] private Transform lookTarget;
    private void OnEnable()
    {
        transform.rotation = Quaternion.LookRotation(lookTarget.position - transform.position);
    }
}
