using UnityEngine;

namespace HiddenResidue.Player

{

    public class CameraFollow : MonoBehaviour

    {

        [Header("Target")]

        [SerializeField] private Transform target;

        [Header("Follow Settings")]

        [SerializeField] private float smoothTime = 0.15f;

        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

        [Header("Camera Boundary")]

        [SerializeField] private bool useBoundary = true;

        [SerializeField] private float left = -10f;

        [SerializeField] private float right = 10f;

        [SerializeField] private float top = 10f;

        [SerializeField] private float bottom = -10f;

        private Vector3 velocity = Vector3.zero;

        private void Start()

        {

            if (target == null)

            {

                GameObject player = GameObject.FindGameObjectWithTag("Player");

                if (player != null)

                    target = player.transform;

            }

        }

        private void LateUpdate()

        {

            if (target == null) return;

            Vector3 desiredPosition = target.position + offset;

            if (useBoundary)

            {

                desiredPosition.x = Mathf.Clamp(desiredPosition.x, left, right);

                desiredPosition.y = Mathf.Clamp(desiredPosition.y, bottom, top);

            }

            transform.position = Vector3.SmoothDamp(

                transform.position,

                desiredPosition,

                ref velocity,

                smoothTime

            );

        }

        public void SetTarget(Transform newTarget)

        {

            target = newTarget;

        }

        public void SnapToTarget()

        {

            if (target == null) return;

            transform.position = target.position + offset;

        }

    }

}