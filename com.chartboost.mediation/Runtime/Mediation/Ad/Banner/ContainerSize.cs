namespace Chartboost.Mediation.Ad.Banner
{
    /// <summary>
    /// Represents a container size with options for fixed, wrapped, or content-based sizing.
    /// </summary>
    public struct ContainerSize
    {
        /// <summary>
        /// Gets the width of the container.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the height of the container.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Initializes a new instance of the ContainerSize struct with specified width and height.
        /// </summary>
        /// <param name="width">The width of the container. Use -1 for wrap content horizontally.</param>
        /// <param name="height">The height of the container. Use -1 for wrap content vertically.</param>
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
    }
}
