using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProject4
{
    /// <summary>
    /// struct representing circular bounds
    /// </summary>
    public struct BoundingCircle
    {
        /// <summary>
        /// the center of the bounding circle
        /// </summary>
        public Vector2 Center;

        /// <summary>
        /// the radius of the bounding circle
        /// </summary>
        public float Radius;

        /// <summary>
        /// constructs a new bounding circle
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public BoundingCircle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        /// Tests for collision between this and another bounding circle
        /// </summary>
        /// <param name="other">other bounding circle</param>
        /// <returns>true for collision, false otherwise</returns>
        public bool CollidesWith(BoundingCircle other)
        {
            return CollisionHelper.Collides(this, other);
        }

        public bool CollidesWith(BoundingRectangle other)
        {
            return CollisionHelper.Collides(other, this);
        }
    }
}
