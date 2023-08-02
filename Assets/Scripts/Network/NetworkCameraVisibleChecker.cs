using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Network
{
    public class NetworkCameraVisibleChecker : NetworkBehaviour
    {
        [SerializeField] private float updatePeriod = .1f;
        private float timer;
        private NetworkIdentity networkIdentity;
        private NetworkIdentity cameraIdentity;
        [SerializeField] private Camera cam;


        private void Start()
        {
            networkIdentity = GetComponent<NetworkIdentity>();
            cameraIdentity = cam.GetComponent<NetworkIdentity>();
        }

        private void Update()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (Time.time - timer > updatePeriod)
            {
                cam ??= Camera.current;
                cameraIdentity ??= cam.GetComponent<NetworkIdentity>();

                networkIdentity.RebuildObservers(false);    
            }
        }

        public override bool OnCheckObserver(NetworkConnection conn)
        {
            return cam.Visible(GetComponent<Collider>());
        }

        public override bool OnRebuildObservers(HashSet<NetworkConnection> observers, bool initialize)
        {
            return false;
        }
        public override void OnSetLocalVisibility(bool vis)
        {
            SetVis(gameObject, vis);
        }

        private static void SetVis(GameObject go, bool vis)
        {
            foreach (var r in go.GetComponents<Renderer>())
            {
                r.enabled = vis;
            }
            for (int i = 0; i < go.transform.childCount; i++)
            {
                var t = go.transform.GetChild(i);
                SetVis(t.gameObject, vis);
            }
        }

    }
}
