// MeanCurvatureFlow.cs
// Discrete Differential Geometry - Mean Curvature Flow
// Added by Graph Technologies, 2025
// Description: Implicit integration scheme for mesh smoothing

using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using DDGCompanion.Core;

namespace DDGCompanion.Algorithms
{
    public class MeanCurvatureFlow
    {
        /// <summary>
        /// Perform one step of implicit mean curvature flow.
        /// System: (M - t*L) * X_new = M * X_old
        /// </summary>
        /// <param name="mesh">Input mesh</param>
        /// <param name="timestep">Integration timestep</param>
        public static void Step(Mesh mesh, double timestep)
        {
            var L = CotanLaplacian.Build(mesh);
            var M = CotanLaplacian.BuildMassMatrix(mesh);
            
            // Build system matrix: A = M - t*L
            var A = M.Subtract(L.Multiply(timestep));
            
            // Get current positions as matrix
            var X = GetPositionMatrix(mesh);
            var b = M.Multiply(X);
            
            // Solve system
            var X_new = A.Solve(b);
            
            // Update mesh positions
            SetPositionMatrix(mesh, X_new);
        }
        
        /// <summary>
        /// Run multiple steps of mean curvature flow.
        /// </summary>
        public static void Flow(Mesh mesh, double timestep, int numSteps)
        {
            for (int i = 0; i < numSteps; i++)
            {
                Step(mesh, timestep);
            }
        }
        
        private static Matrix<double> GetPositionMatrix(Mesh mesh)
        {
            var positions = mesh.GetPositions();
            var X = Matrix<double>.Build.Dense(positions.Length, 3);
            
            for (int i = 0; i < positions.Length; i++)
            {
                X[i, 0] = positions[i].X;
                X[i, 1] = positions[i].Y;
                X[i, 2] = positions[i].Z;
            }
            
            return X;
        }
        
        private static void SetPositionMatrix(Mesh mesh, Matrix<double> X)
        {
            var positions = new System.Numerics.Vector3[X.RowCount];
            
            for (int i = 0; i < X.RowCount; i++)
            {
                positions[i] = new System.Numerics.Vector3(
                    (float)X[i, 0],
                    (float)X[i, 1],
                    (float)X[i, 2]
                );
            }
            
            mesh.SetPositions(positions);
        }
    }
}
