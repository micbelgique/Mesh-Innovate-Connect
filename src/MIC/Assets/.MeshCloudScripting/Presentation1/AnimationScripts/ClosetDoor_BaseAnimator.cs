//
// This file was auto-generated from Animator assets in Unity Project MIC.
//

// <auto-generated />

namespace MeshApp.Animations
{
    using System;
    using Microsoft.Mesh.CloudScripting;
    using Microsoft.Mesh.CloudScripting.Declarations;

    [UserCreatable(false)]
    public class ClosetDoor_BaseAnimator : AnimationNode
    {
        private readonly float[] _baseLayerSpeeds = { 1F, 1F, 1F, };

        protected ClosetDoor_BaseAnimator(in Guid ahandle, bool transfer)
        : base(ahandle, transfer)
        { }

        public enum BaseLayerState
        {
            ClosetClosing,
            Idle,
            ClosetOpening,
        }

        [Replication(ReplicationKind.Full)]
        public BaseLayerState CurrentBaseLayerState
        {
            get => (BaseLayerState)((int)this[nameof(CurrentBaseLayerState)]);
            set
            {
                this[nameof(CurrentBaseLayerState)].SetValue((int)value);
                SystemTimeOfBaseLayerUpdated = Application.ToServerTime(DateTime.UtcNow).Ticks;
            }
        }

        public float BaseLayerSpeed
            => _baseLayerSpeeds[(int)this[nameof(CurrentBaseLayerState)]];

        [Replication(ReplicationKind.Full)]
        [Serialized(false)]
        internal long SystemTimeOfBaseLayerUpdated
        {
            get => (long)GetPropertyAccessor(nameof(SystemTimeOfBaseLayerUpdated));
            set => GetPropertyAccessor(nameof(SystemTimeOfBaseLayerUpdated)).SetValue(value);
        }

        internal static ClosetDoor_BaseAnimator GetOrCreateInstance(in Guid cookie, bool transfer)
        {
            var result = GetOrCreateCloudObject(
                cookie,
                transfer,
                (handle, t) => new ClosetDoor_BaseAnimator(handle, transfer: t));

            return result as ClosetDoor_BaseAnimator;
        }
    }
}