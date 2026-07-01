using UnityEngine;

namespace ZillRunner.CameraSystem
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 6f, -10f);
        [SerializeField] private float smoothSpeed = 6f;
        [SerializeField] private float lookAhead = 8f;

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desired = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);

            Vector3 lookTarget = target.position + Vector3.forward * lookAhead;
            transform.LookAt(lookTarget);
        }

        public void SetTarget(Transform newTarget) => target = newTarget;
    }
}
