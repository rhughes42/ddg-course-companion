// LaplacianSmoothing.cs
// Example: Mesh smoothing using mean curvature flow
// Added by Graph Technologies, 2025

using System;
using System.Numerics;
using DDGCompanion.Core;
using DDGCompanion.Algorithms;

namespace DDGCompanion.Examples
{
    public class LaplacianSmoothing
    {
        public static void Run()
        {
            Console.WriteLine("DDG Example: Laplacian Smoothing");
            Console.WriteLine("================================\n");
            
            // Create simple noisy mesh (tetrahedron with noise)
            var positions = new Vector3[]
            {
                new(0, 0, 0),
                new(1, 0, 0),
                new(0.5f, 0.866f, 0),
                new(0.5f, 0.289f, 0.816f)
            };
            
            // Add random noise
            var random = new Random(42);
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] += new Vector3(
                    (float)(random.NextDouble() - 0.5) * 0.1f,
                    (float)(random.NextDouble() - 0.5) * 0.1f,
                    (float)(random.NextDouble() - 0.5) * 0.1f
                );
            }
            
            var faces = new int[,]
            {
                { 0, 1, 2 },
                { 0, 1, 3 },
                { 0, 2, 3 },
                { 1, 2, 3 }
            };
            
            var mesh = new Mesh();
            mesh.Build(positions, faces);
            mesh.Normalize();
            
            Console.WriteLine("Initial mesh:");
            Console.WriteLine($"  Vertices: {mesh.Vertices.Count}");
            Console.WriteLine($"  Euler characteristic: {mesh.EulerCharacteristic()}\n");
            
            // Run mean curvature flow
            Console.WriteLine("Running mean curvature flow...");
            double timestep = 0.001;
            int numSteps = 50;
            
            for (int i = 0; i < numSteps; i++)
            {
                MeanCurvatureFlow.Step(mesh, timestep);
                
                if (i % 10 == 0)
                {
                    Console.WriteLine($"  Step {i} complete");
                }
            }
            
            Console.WriteLine("\nSmoothing complete!");
            Console.WriteLine("Mesh has been smoothed (high-frequency noise removed)");
        }
    }
}
