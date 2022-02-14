using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public interface IHyperplane
    {
        public float DistanceTo(Vector4 vertex);

        /// <summary>
        /// Returns a point, that corresponds to A line crossection with a Hyperplane offseted by given amount
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Vector3 CrossSectionWithLine(Vector4 a, Vector4 b, float offset);
    }
}
