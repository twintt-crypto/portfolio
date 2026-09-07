using UnityEngine;

namespace S7
{
    public class MinimapIcon : MonoBehaviour
    {
        private FieldMinimap _minimap;

        public void Initialize(FieldMinimap minimap)
        {
            _minimap = minimap;
            _minimap.RegisterIcon(this);
        }

        public void ResetRotation()
        {
            transform.rotation = Quaternion.identity;
        }

        private void OnDestroy()
        {
            if (_minimap != null) _minimap.UnregisterIcon(this);
        }
    }
}
