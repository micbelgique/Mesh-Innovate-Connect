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
    public class OpenandcloseAnimator : AnimationNode
    {
        private readonly float[] _baseLayerSpeeds = { 1F, 1F, 1F, };

        protected OpenandcloseAnimator(in Guid ahandle, bool transfer)
        : base(ahandle, transfer)
        { }

        public enum BaseLayerState
        {
            Closing,
            Opening,
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

        internal static OpenandcloseAnimator GetOrCreateInstance(in Guid cookie, bool transfer)
        {
            var result = GetOrCreateCloudObject(
                cookie,
                transfer,
                (handle, t) => new OpenandcloseAnimator(handle, transfer: t));

            return result as OpenandcloseAnimator;
        }
    }
}