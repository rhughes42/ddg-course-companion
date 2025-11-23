// ConformalMapping.cs
// Example: Surface parameterization using conformal maps
// Added by Graph Technologies, 2025

using System;
using System.Numerics;
using DDGCompanion.Core;
using DDGCompanion.Algorithms;

namespace DDGCompanion.Examples
{
    public class ConformalMapping
    {
        public static void Run()
        {
            Console.WriteLine("DDG Example: Conformal Parameterization");
            Console.WriteLine("======================================\n");
            
            // Create simple mesh
            var positions = new Vector3[]
            {
                new(0, 0, 0),
                new(1, 0, 0),
                new(2, 0, 0),
                new(0, 1, 0),
                new(1, 1, 0),
                new(2, 1, 0)
            };
            
            var faces = new int[,]
            {
                { 0, 1, 3 },
                { 1, 4, 3 },
                { 1, 2, 4 },
                { 2, 5, 4 }
            };
            
            var mesh = new Mesh();
            mesh.Build(positions, faces);
            
            Console.WriteLine("Computing spectral conformal parameterization...");
            
            // Compute conformal UV coordinates
            var uv = ConformalParameterization.Spectral(mesh);
            
            Console.WriteLine("\nUV Coordinates:");
            for (int i = 0; i < uv.Length; i++)
            {
                Console.WriteLine($"  v{i}: u = {uv[i].X:F6}, v = {uv[i].Y:F6}");
            }
            
            // Compute Dirichlet energy
            double energy = ConformalParameterization.DirichletEnergy(mesh, uv);
            Console.WriteLine($"\nDirichlet Energy: {energy:F6}");
            Console.WriteLine("(Lower energy = less distortion)");
        }
    }
}
