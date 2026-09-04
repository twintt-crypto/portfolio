using UnityEngine;

namespace UI.Field
{
    public class Billboard : MonoBehaviour
    {
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void LateUpdate()
        {
            if (_camera == null) return;
            transform.rotation = _camera.transform.rotation;
        }
    }
}
