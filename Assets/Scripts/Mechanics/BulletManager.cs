using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class BulletManager : MonoBehaviour
{
    public NetworkHash128 AssetId => assetId;

    [SerializeField] private GameObject bulletPrefab;
    private NetworkHash128 assetId;

    void Start()
    {
        assetId = bulletPrefab.GetComponent<NetworkIdentity>().assetId;
        ClientScene.RegisterSpawnHandler(assetId, SpawnBullet, UnSpawnBullet);
    }

    public GameObject SpawnBullet(Vector3 position)
    {
        return Instantiate(bulletPrefab, position, Quaternion.identity);
    }

    public GameObject SpawnBullet(Vector3 position, NetworkHash128 assetId)
    {
        return SpawnBullet(position);
    }

    public void UnSpawnBullet(GameObject spawned)
    {
        Destroy(spawned);
    }
}

public class Player : NetworkBehaviour
{
    private BulletManager bulletManager;

    private void Start()
    {
        bulletManager = GameObject.Find("BulletManager").GetComponent<BulletManager>();
    }

    private void Update()
    {
        if (!isLocalPlayer || !hasAuthority)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdFire();
        }
    }

    [Command]
    private void CmdFire()
    {
        var bullet = bulletManager.SpawnBullet(transform.position + transform.forward);
        bullet.GetComponent<Rigidbody>().velocity = transform.forward * 4;

        NetworkServer.Spawn(bullet, bulletManager.AssetId);

        StartCoroutine(Destroy(bullet, 2.0f));
    }

    private IEnumerator Destroy(GameObject go, float timer)
    {
        yield return new WaitForSeconds(timer);
        bulletManager.UnSpawnBullet(go);
        NetworkServer.UnSpawn(go);
    }
}