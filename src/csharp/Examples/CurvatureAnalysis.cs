// CurvatureAnalysis.cs
// Example: Computing and analyzing discrete Gaussian curvature
// Added by Graph Technologies, 2025

using System;
using System.Linq;
using System.Numerics;
using DDGCompanion.Core;
using DDGCompanion.Algorithms;

namespace DDGCompanion.Examples
{
    public class CurvatureAnalysis
    {
        public static void Run()
        {
            Console.WriteLine("DDG Example: Curvature Analysis");
            Console.WriteLine("================================\n");
            
            // Create cube mesh
            var positions = new Vector3[]
            {
                new(-1, -1, -1), new(1, -1, -1),
                new(1,  1, -1), new(-1,  1, -1),
                new(-1, -1,  1), new(1, -1,  1),
                new(1,  1,  1), new(-1,  1,  1)
            };
            
            var faces = new int[,]
            {
                { 0, 1, 2 }, { 0, 2, 3 },  // Front
                { 4, 7, 6 }, { 4, 6, 5 },  // Back
                { 0, 4, 5 }, { 0, 5, 1 },  // Bottom
                { 2, 6, 7 }, { 2, 7, 3 },  // Top
                { 0, 3, 7 }, { 0, 7, 4 },  // Left
                { 1, 5, 6 }, { 1, 6, 2 }   // Right
            };
            
            var mesh = new Mesh();
            mesh.Build(positions, faces);
            
            Console.WriteLine("Mesh: Cube");
            Console.WriteLine($"  V = {mesh.Vertices.Count}, E = {mesh.Edges.Count}, F = {mesh.Faces.Count}");
            Console.WriteLine($"  χ = {mesh.EulerCharacteristic()}\n");
            
            // Compute Gaussian curvature
            var K = DiscreteGaussianCurvature.Compute(mesh);
            
            Console.WriteLine("Gaussian Curvature per vertex:");
            for (int i = 0; i < K.Length; i++)
            {
                double degrees = K[i] * 180.0 / Math.PI;
                Console.WriteLine($"  v[{i}]: K = {K[i]:F6} ({degrees:F2}°)");
            }
            
            // Verify Gauss-Bonnet
            double totalK = K.Sum();
            double expectedK = 2.0 * Math.PI * mesh.EulerCharacteristic();
            double error = Math.Abs(totalK - expectedK);
            
            Console.WriteLine("\nGauss-Bonnet Verification:");
            Console.WriteLine($"  Total curvature: {totalK:F6}");
            Console.WriteLine($"  Expected (2πχ):   {expectedK:F6}");
            Console.WriteLine($"  Error: {error:F6}");
            
            if (error < 0.01)
            {
                Console.WriteLine("  ✓ Gauss-Bonnet theorem verified!");
            }
            else
            {
                Console.WriteLine("  ✗ Error exceeds tolerance");
            }
        }
    }
}
