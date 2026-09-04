using UnityEngine;
using VContainer;
using VContainer.Unity;
using S7.Game.Field;

namespace S7
{
    public class FieldLifeTimeScope : LifetimeScope
    {
        [SerializeField] private Transform player;

        protected override void Configure(IContainerBuilder builder)
        {
            // mono
            builder.RegisterComponentInHierarchy<FieldManager>();
            builder.RegisterComponentInHierarchy<SceneField>(); 
            builder.RegisterComponentInHierarchy<FieldCameraController>();
        }
    }
}

