using Unity.Cinemachine;
using UnityEngine;

namespace S7.Game.Field
{
    public class FieldCameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineOrbitalFollow _orbital;

        public void AlignToTarget()
        {
            if (_orbital == null) return;
            Transform follow = _orbital.GetComponent<CinemachineCamera>().Follow;
            if (follow == null) return;
            _orbital.HorizontalAxis.Value = follow.eulerAngles.y;
        }
    }
}
