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
    public class LowPolyHumanAnimatorAnimator : AnimationNode
    {
        private readonly float[] _baseLayerSpeeds = { 1F, };

        protected LowPolyHumanAnimatorAnimator(in Guid ahandle, bool transfer)
        : base(ahandle, transfer)
        { }

        public enum BaseLayerState
        {
            Idle,
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

        internal static LowPolyHumanAnimatorAnimator GetOrCreateInstance(in Guid cookie, bool transfer)
        {
            var result = GetOrCreateCloudObject(
                cookie,
                transfer,
                (handle, t) => new LowPolyHumanAnimatorAnimator(handle, transfer: t));

            return result as LowPolyHumanAnimatorAnimator;
        }
    }
}