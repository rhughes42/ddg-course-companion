// MeshValidator.cs
// Mesh validation utilities
// Added by Graph Technologies, 2025
// Description: Validate mesh topology and geometry

using System;
using System.Linq;
using DDGCompanion.Core;

namespace DDGCompanion.Utils
{
    public static class MeshValidator
    {
        public class ValidationResult
        {
            public bool IsValid { get; set; }
            public List<string> Errors { get; set; } = new();
            public List<string> Warnings { get; set; } = new();
        }
        
        /// <summary>
        /// Validate mesh structure and topology.
        /// </summary>
        public static ValidationResult Validate(Mesh mesh)
        {
            var result = new ValidationResult { IsValid = true };
            
            // Check halfedge connectivity
            foreach (var he in mesh.HalfEdges)
            {
                if (he.Twin?.Twin != he)
                {
                    result.Errors.Add($"HalfEdge {he.Index}: Twin symmetry broken");
                    result.IsValid = false;
                }
                
                if (he.Next?.Next?.Next != he)
                {
                    result.Errors.Add($"HalfEdge {he.Index}: Cycle property broken");
                    result.IsValid = false;
                }
                
                if (he.Edge != he.Twin?.Edge)
                {
                    result.Errors.Add($"HalfEdge {he.Index}: Edge consistency broken");
                    result.IsValid = false;
                }
            }
            
            // Check mesh is manifold
            foreach (var v in mesh.Vertices)
            {
                if (v.HalfEdge == null)
                {
                    result.Warnings.Add($"Vertex {v.Index}: Isolated (no halfedge)");
                }
            }
            
            // Check for degenerate faces
            foreach (var f in mesh.Faces)
            {
                double area = f.Area();
                if (area < 1e-10)
                {
                    result.Warnings.Add($"Face {f.Index}: Degenerate (area ≈ 0)");
                }
            }
            
            // Verify indices
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                if (mesh.Vertices[i].Index != i)
                {
                    result.Errors.Add($"Vertex index mismatch at position {i}");
                    result.IsValid = false;
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// Print validation results.
        /// </summary>
        public static void PrintResults(ValidationResult result)
        {
            if (result.IsValid)
            {
                Console.WriteLine("✓ Mesh is valid");
            }
            else
            {
                Console.WriteLine("✗ Mesh validation failed");
            }
            
            if (result.Errors.Any())
            {
                Console.WriteLine("\nErrors:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"  - {error}");
                }
            }
            
            if (result.Warnings.Any())
            {
                Console.WriteLine("\nWarnings:");
                foreach (var warning in result.Warnings)
                {
                    Console.WriteLine($"  - {warning}");
                }
            }
        }
        
        /// <summary>
        /// Print mesh statistics.
        /// </summary>
        public static void PrintStatistics(Mesh mesh)
        {
            Console.WriteLine("Mesh Statistics:");
            Console.WriteLine("===============");
            Console.WriteLine($"  Vertices: {mesh.Vertices.Count}");
            Console.WriteLine($"  Edges: {mesh.Edges.Count}");
            Console.WriteLine($"  Faces: {mesh.Faces.Count}");
            Console.WriteLine($"  HalfEdges: {mesh.HalfEdges.Count}");
            Console.WriteLine($"  Euler characteristic: {mesh.EulerCharacteristic()}");
            
            int genus = (2 - mesh.EulerCharacteristic()) / 2;
            Console.WriteLine($"  Genus: {genus}");
            
            int boundaryVerts = mesh.Vertices.Count(v => v.IsBoundary());
            Console.WriteLine($"  Boundary vertices: {boundaryVerts}");
            
            double avgDegree = mesh.Vertices.Average(v => v.Degree());
            Console.WriteLine($"  Average degree: {avgDegree:F2}");
        }
    }
}
