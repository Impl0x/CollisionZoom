namespace CollisionZoom
{
    class Render
    {
        /// <summary>
        /// Normalizes a given view transform so that there
        /// is no stretching or skewing when applied to a 
        /// viewport of a given aspect ratio.
        /// </summary>
        public static Transform NormalizeView(double aspectRatio, Transform view)
        {
            if (aspectRatio < 1.0) return (Transform.Scale(aspectRatio, 1.0) * view);
            else return (Transform.Scale(1.0, 1.0 / aspectRatio) * view);
        }
    }
}
