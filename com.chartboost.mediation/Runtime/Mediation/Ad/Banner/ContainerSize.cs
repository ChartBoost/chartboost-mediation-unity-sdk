using System;
using Newtonsoft.Json;

namespace Chartboost.Mediation.Ad.Banner
{
    /// <summary>
    /// Represents a container size with options for fixed, wrapped, or content-based sizing.
    /// </summary>
    public readonly struct ContainerSize : IEquatable<ContainerSize>
    {
        /// <summary>
        /// Gets the width of the container.
        /// </summary>
        [JsonProperty("width")]
        public int Width { get; }

        /// <summary>
        /// Gets the height of the container.
        /// </summary>
        [JsonProperty("height")]
        public int Height { get; }

        /// <summary>
        /// Initializes a new instance of the ContainerSize struct with specified width and height.
        /// </summary>
        /// <param name="width">The width of the container. Use -1 for wrap content horizontally.</param>
        /// <param name="height">The height of the container. Use -1 for wrap content vertically.</param>
        [JsonConstructor]
        public ContainerSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Creates a container size that automatically adjusts its width to wrap its content,
        /// with a fixed height.
        /// </summary>
        /// <param name="height">The fixed height of the container.</param>
        /// <returns>A new ContainerSize instance.</returns>
        public static ContainerSize WrapHorizontal(int height) => new(-1, height);

        /// <summary>
        /// Creates a container size that automatically adjusts its height to wrap its content,
        /// with a fixed width.
        /// </summary>
        /// <param name="width">The fixed width of the container.</param>
        /// <returns>A new ContainerSize instance.</returns>
        public static ContainerSize WrapVertical(int width) => new(width, -1);

        /// <summary>
        /// Creates a container size that wraps both its width and height to fit its content.
        /// </summary>
        /// <returns>A new ContainerSize instance.</returns>
        public static ContainerSize WrapContent() => new(-1, -1);

        /// <summary>
        /// Creates a container size with fixed dimensions.
        /// </summary>
        /// <param name="width">The fixed width of the container.</param>
        /// <param name="height">The fixed height of the container.</param>
        /// <returns>A new ContainerSize instance.</returns>
        public static ContainerSize FixedSize(int width, int height) => new(width, height);

        /// <summary>
        /// Determines whether the specified <see cref="ContainerSize"/> is equal to the current instance.
        /// </summary>
        /// <param name="other">The <see cref="ContainerSize"/> to compare with the current instance.</param>
        /// <returns>true if the specified <see cref="ContainerSize"/> is equal to the current instance; otherwise, false.</returns>
        public bool Equals(ContainerSize other)
        {
            return Width == other.Width && Height == other.Height;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>true if the specified object is a <see cref="ContainerSize"/> and is equal to the current instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj is ContainerSize other && Equals(other);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Width, Height);
        }
    }
}
