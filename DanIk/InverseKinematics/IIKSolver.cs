using System.Collections.Generic;
using System.Numerics;

namespace DanIk.InverseKinematics
{
    /// <summary>
    /// Represents a method of solving IK problems
    /// </summary>
    public interface IIKSolver
    {
        /// <summary>
        /// Calculates the bone transforms to achieve a given end effector target location (in relative space to root bone)
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        IEnumerable<float> Solve(Vector2 target);
    }
}
