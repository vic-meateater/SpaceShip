using Network;
using UnityEngine;

namespace Mechanics
{
    public class PlanetOrbit : NetworkMovableObject
    {
        protected override float speed => smoothTime;

        [SerializeField] private Transform aroundPoint;
        [SerializeField] private float smoothTime = .3f;
        [SerializeField] private float circleInSecond = 1f;

        [SerializeField] private float offsetSin = 1;
        [SerializeField] private float offsetCos = 1;
        [SerializeField] private float rotationSpeed;

        private float dist;
        private float currentAng;
        private Vector3 currentPositionSmoothVelocity;
        private float currentRotationAngle;

        private const float circleRadians = Mathf.PI * 2;

        private void Start()
        {
            if (isServer)
            {
                dist = (transform.position - aroundPoint.position).magnitude;
            }
            Initiate(UpdatePhase.FixedUpdate);
        }

        protected override void HasAuthorityMovement()
        {
            if (!isServer)
            {
                return;
            }

            Vector3 p = aroundPoint.position;
            p.x += Mathf.Sin(currentAng) * dist * offsetSin;
            p.z += Mathf.Cos(currentAng) * dist * offsetCos;
            transform.position = p;
            currentRotationAngle += Time.deltaTime * rotationSpeed;
            currentRotationAngle = Mathf.Clamp(currentRotationAngle, 0, 361);
            if (currentRotationAngle >= 360)
            {
                currentRotationAngle = 0;
            }
            transform.rotation = Quaternion.AngleAxis(currentRotationAngle, transform.up);
            currentAng += circleRadians * circleInSecond * Time.deltaTime;

            SendToServer();
        }

        protected override void SendToServer()
        {
            serverPosition = transform.position;
            serverEuler = transform.eulerAngles;
        }

        protected override void FromServerUpdate()
        {
            if (!isClient)
            {
                return;
            }
            transform.position = Vector3.SmoothDamp(transform.position,
                serverPosition, ref currentPositionSmoothVelocity, speed);
            transform.rotation = Quaternion.Euler(serverEuler);
        }
    }
}
