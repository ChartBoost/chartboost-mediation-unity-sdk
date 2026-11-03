using System;
using System.Threading.Tasks;
using Chartboost.Banners;
using Chartboost.Mediation.Requests;
using UnityEngine;

namespace Chartboost.Mediation.Ad.Banner
{
    /// <summary>
    /// A platform banner whose SDK view lives in a shared <see cref="NativeBannerContainer"/>. The container owns
    /// placement, size, visibility, drag and hosting; the platform bridge owns the ad and its creative.
    /// </summary>
    internal abstract class NativeBannerAdBase : BannerAdBase
    {
        private NativeBannerContainer _container;
        private Vector2 _position;
        private Vector2 _pivot;
        private bool _visible = true;
        private bool _draggable;
        private ContainerSize? _containerSize; // null until set: wraps and clips

        protected NativeBannerAdBase(IntPtr uniqueId) : base(uniqueId) { }

        /// <summary>Set once by the platform constructor; pushes the state set so far.</summary>
        protected NativeBannerContainer Container
        {
            get => _container;
            set
            {
                _container = value;
                if (value == null)
                    return;
                value.SetAbsolute(_position, _pivot);
                value.Visible = _visible;
                value.Draggable = _draggable;
                value.DragBegan += (_, origin) => OnDragBegin(origin.x, origin.y);
                value.Dragged += (_, origin) => OnDrag(origin.x, origin.y);
                value.DragEnded += OnContainerDragEnded;
            }
        }

        /// <inheritdoc />
        public override Vector2 Position
        {
            get => _position;
            set
            {
                if (_position == value)
                    return;
                _position = value;
                Container?.SetAbsolute(_position, _pivot);
            }
        }

        /// <inheritdoc />
        public override Vector2 Pivot
        {
            get => _pivot;
            set
            {
                if (_pivot == value)
                    return;
                _pivot = value;
                Container?.SetAbsolute(_position, _pivot);
            }
        }

        /// <inheritdoc />
        public override bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                if (Container != null)
                    Container.Visible = value;
            }
        }

        /// <inheritdoc />
        public override bool Draggable
        {
            get => _draggable;
            set
            {
                _draggable = value;
                if (Container != null)
                    Container.Draggable = value;
            }
        }

        /// <inheritdoc />
        /// <remarks>A wrapping axis reads back as the loaded creative's size.</remarks>
        public override ContainerSize ContainerSize
        {
            get
            {
                var size = _containerSize ?? ContainerSize.WrapContent();
                if (size.Width >= 0 && size.Height >= 0)
                    return size;
                var creative = BannerSize; // native call: only when an axis wraps
                var width = size.Width < 0 ? (int)(creative?.Width ?? 0) : size.Width;
                var height = size.Height < 0 ? (int)(creative?.Height ?? 0) : size.Height;
                return new ContainerSize(width, height);
            }
            set
            {
                if (_containerSize.Equals(value))
                    return;
                _containerSize = value;
                var wraps = value.Width < 0 || value.Height < 0;
                if (Container != null)
                {
                    Container.SetSize(value.Width < 0 ? NativeBannerContainer.Wrap : value.Width,
                        value.Height < 0 ? NativeBannerContainer.Wrap : value.Height);
                    // Match Ad lets a wide creative draw past the container instead of clipping it.
                    Container.ClipContent = !wraps;
                }
                SetContentSize(value);
            }
        }

        /// <summary>Sizes the SDK view inside the container: fixed axes match it, wrapping axes wrap the creative.</summary>
        protected abstract void SetContentSize(ContainerSize size);

        /// <summary>Attaches the container before the first load, so a host set earlier is used directly.</summary>
        public override Task<BannerAdLoadResult> Load(BannerAdLoadRequest request)
        {
            Container?.Attach();
            return base.Load(request);
        }

        /// <summary>Automation hook: hosts the banner in <paramref name="host"/>, or back at its position with none.</summary>
        internal void SetHost(NativeView host) => Container?.SetHost(host);

        /// <inheritdoc />
        protected sealed override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            IsDisposed = true;
            // The container releases the SDK view before the bridge destroys the ad.
            if (disposing)
            {
                Container?.Dispose();
                DestroyNative(true);
                return;
            }
            // Finalizer thread: run both steps, in order, on the main thread.
            var container = Container is { CanDisposeFromFinalizer: true } ? Container : null;
            MainThreadDispatcher.Post(_ =>
            {
                container?.Dispose();
                DestroyNative(false);
            });
        }

        /// <summary>Destroys the native ad, after the container has released its view.</summary>
        protected abstract void DestroyNative(bool disposing);

        // A drop keeps the dragged spot, so later layout and refreshes use it.
        private void OnContainerDragEnded(NativeBannerContainer container, Vector2 origin)
        {
            _position = container.Position;
            OnDragEnd(origin.x, origin.y);
        }
    }
}
