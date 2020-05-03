// Copyright (c) Six Labors and contributors.
// Licensed under the GNU Affero General Public License, Version 3.

using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors
{
    /// <summary>
    /// Implements an algorithm to alter the pixels of a cloned image.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    public interface ICloningImageProcessor<TPixel> : IImageProcessor<TPixel>
        where TPixel : unmanaged, IPixel<TPixel>
    {
        /// <summary>
        /// Clones the specified <see cref="Image{TPixel}"/> and executes the process against the clone.
        /// </summary>
        /// <returns>The <see cref="Image{TPixel}"/>.</returns>
        Image<TPixel> CloneAndExecute();
    }
}
