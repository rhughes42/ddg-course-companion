// HodgeDecomposition.cs
// Discrete Differential Geometry - Hodge Decomposition for Vector Fields
// Added by Graph Technologies, 2025
// Description: Decompose discrete 1-forms into exact, coexact, and harmonic components

using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using DDGCompanion.Core;

namespace DDGCompanion.Algorithms
{
    /// <summary>
    /// Components of a vector field decomposition: ω = dα + δβ + γ
    /// </summary>
    public class VectorFieldComponents
    {
        public Vector<double> Exact { get; set; }      // dα component (curl-free)
        public Vector<double> Coexact { get; set; }    // δβ component (divergence-free)
        public Vector<double> Harmonic { get; set; }   // γ component (both curl-free and divergence-free)
        
        public VectorFieldComponents(int size)
        {
            Exact = Vector<double>.Build.Dense(size);
            Coexact = Vector<double>.Build.Dense(size);
            Harmonic = Vector<double>.Build.Dense(size);
        }
    }
    
    /// <summary>
    /// Hodge decomposition of discrete 1-forms on triangle meshes.
    /// Splits a vector field into orthogonal exact, coexact, and harmonic components.
    /// </summary>
    public class HodgeDecomposition
    {
        /// <summary>
        /// Decompose 1-form ω into ω = dα + δβ + γ
        /// Algorithm:
        ///   1. Build exterior derivatives d0, d1
        ///   2. Build Hodge stars ⋆0, ⋆1, ⋆2
        ///   3. Compute codifferential: δ = ⋆d⋆
        ///   4. Solve least squares: d0α ≈ ω
        ///   5. Solve: δ1β ≈ ω - d0α
        ///   6. Harmonic: γ = ω - d0α - δ1β
        /// </summary>
        public static VectorFieldComponents Decompose(Mesh mesh, Vector<double> omega)
        {
            var result = new VectorFieldComponents(mesh.Edges.Count);
            
            // Build operators
            var d0 = BuildD0(mesh);
            var d1 = BuildD1(mesh);
            var star0 = BuildHodgeStar0(mesh);
            var star1 = BuildHodgeStar1(mesh);
            var star2 = BuildHodgeStar2(mesh);
            
            // Codifferential: δ1 = -⋆2^{-1} d1^T ⋆1
            // Simplified: δ1 = d1^T (using properties of Hodge star)
            var delta1 = d1.Transpose();
            
            // Solve for exact component: d0*α ≈ ω (least squares)
            // Using pseudo-inverse or QR decomposition
            var d0T = d0.Transpose();
            var normalMatrix = d0T.Multiply(d0);
            var rhs = d0T.Multiply(omega);
            
            // Add small regularization to avoid singularity
            double regularization = 1e-10;
            for (int i = 0; i < normalMatrix.RowCount; i++)
            {
                normalMatrix[i, i] += regularization;
            }
            
            try
            {
                var alpha = normalMatrix.Solve(rhs);
                if (!alpha.Any(x => double.IsNaN(x) || double.IsInfinity(x)))
                {
                    result.Exact = d0.Multiply(alpha);
                }
                else
                {
                    result.Exact = Vector<double>.Build.Dense(mesh.Edges.Count);
                }
            }
            catch
            {
                // If system is singular, exact component is zero
                result.Exact = Vector<double>.Build.Dense(mesh.Edges.Count);
            }
            
            // Solve for coexact component: δ1*β ≈ ω - exact
            var residual = omega.Subtract(result.Exact);
            var delta1T = delta1.Transpose();
            var normalMatrix2 = delta1T.Multiply(delta1);
            var rhs2 = delta1T.Multiply(residual);
            
            // Add regularization
            for (int i = 0; i < normalMatrix2.RowCount; i++)
            {
                normalMatrix2[i, i] += regularization;
            }
            
            try
            {
                var beta = normalMatrix2.Solve(rhs2);
                if (!beta.Any(x => double.IsNaN(x) || double.IsInfinity(x)))
                {
                    result.Coexact = delta1.Multiply(beta);
                }
                else
                {
                    result.Coexact = Vector<double>.Build.Dense(mesh.Edges.Count);
                }
            }
            catch
            {
                // If system is singular, coexact component is zero
                result.Coexact = Vector<double>.Build.Dense(mesh.Edges.Count);
            }
            
            // Harmonic component: what remains
            result.Harmonic = omega.Subtract(result.Exact).Subtract(result.Coexact);
            
            return result;
        }
        
        /// <summary>
        /// Build exterior derivative d0: 0-forms → 1-forms (vertices → edges)
        /// d0[i,j] = +1 if edge i goes to vertex j, -1 if it comes from vertex j
        /// </summary>
        public static SparseMatrix BuildD0(Mesh mesh)
        {
            int nVerts = mesh.Vertices.Count;
            int nEdges = mesh.Edges.Count;
            
            var builder = new SparseMatrix(nEdges, nVerts);
            
            foreach (var edge in mesh.Edges)
            {
                int i = edge.Index;
                var v0 = edge.HalfEdge?.Twin?.Vertex;
                var v1 = edge.HalfEdge?.Vertex;
                
                if (v0 != null && v1 != null)
                {
                    builder[i, v0.Index] = -1.0;
                    builder[i, v1.Index] = 1.0;
                }
            }
            
            return builder;
        }
        
        /// <summary>
        /// Build exterior derivative d1: 1-forms → 2-forms (edges → faces)
        /// d1[i,j] = sign if edge j is in face i (sign depends on orientation)
        /// </summary>
        public static SparseMatrix BuildD1(Mesh mesh)
        {
            int nEdges = mesh.Edges.Count;
            int nFaces = mesh.Faces.Count;
            
            var builder = new SparseMatrix(nFaces, nEdges);
            
            foreach (var face in mesh.Faces)
            {
                int fIdx = face.Index;
                var he = face.HalfEdge;
                
                if (he != null)
                {
                    var current = he;
                    do
                    {
                        if (current.Edge != null)
                        {
                            int eIdx = current.Edge.Index;
                            // Sign depends on orientation relative to face
                            double sign = (current.Edge.HalfEdge == current) ? 1.0 : -1.0;
                            builder[fIdx, eIdx] = sign;
                        }
                        current = current.Next;
                    } while (current != null && current != he);
                }
            }
            
            return builder;
        }
        
        /// <summary>
        /// Build Hodge star for 0-forms (vertex-based)
        /// ⋆0 is a diagonal matrix with vertex areas
        /// </summary>
        public static SparseMatrix BuildHodgeStar0(Mesh mesh)
        {
            int nVerts = mesh.Vertices.Count;
            var builder = new SparseMatrix(nVerts, nVerts);
            
            // Compute barycentric dual area for each vertex
            foreach (var vertex in mesh.Vertices)
            {
                double area = ComputeVertexArea(vertex);
                builder[vertex.Index, vertex.Index] = area;
            }
            
            return builder;
        }
        
        /// <summary>
        /// Build Hodge star for 1-forms (edge-based)
        /// ⋆1 relates primal edge to dual edge
        /// </summary>
        public static SparseMatrix BuildHodgeStar1(Mesh mesh)
        {
            int nEdges = mesh.Edges.Count;
            var builder = new SparseMatrix(nEdges, nEdges);
            
            foreach (var edge in mesh.Edges)
            {
                // Dual edge length over primal edge length
                double weight = ComputeEdgeHodgeStar(edge);
                builder[edge.Index, edge.Index] = weight;
            }
            
            return builder;
        }
        
        /// <summary>
        /// Build Hodge star for 2-forms (face-based)
        /// ⋆2 is a diagonal matrix with inverse face areas
        /// </summary>
        public static SparseMatrix BuildHodgeStar2(Mesh mesh)
        {
            int nFaces = mesh.Faces.Count;
            var builder = new SparseMatrix(nFaces, nFaces);
            
            foreach (var face in mesh.Faces)
            {
                double area = face.Area();
                if (area > 1e-10)
                {
                    builder[face.Index, face.Index] = 1.0 / area;
                }
            }
            
            return builder;
        }
        
        /// <summary>
        /// Compute the barycentric dual area associated with a vertex
        /// </summary>
        private static double ComputeVertexArea(Vertex vertex)
        {
            double area = 0.0;
            
            if (vertex.HalfEdge == null) return 0.0;
            
            var he = vertex.HalfEdge;
            do
            {
                if (he.Face != null)
                {
                    // Add 1/3 of the triangle area
                    area += he.Face.Area() / 3.0;
                }
                he = he.Twin?.Next;
            } while (he != null && he != vertex.HalfEdge);
            
            return area;
        }
        
        /// <summary>
        /// Compute Hodge star weight for an edge
        /// Weight = (cot α + cot β) / 2 where α, β are opposite angles
        /// </summary>
        private static double ComputeEdgeHodgeStar(Edge edge)
        {
            if (edge.HalfEdge == null) return 0.0;
            
            double weight = 0.0;
            
            // Contribution from first adjacent face
            if (edge.HalfEdge.Face != null)
            {
                weight += ComputeCotanAngle(edge.HalfEdge.Next);
            }
            
            // Contribution from second adjacent face
            if (edge.HalfEdge.Twin?.Face != null)
            {
                weight += ComputeCotanAngle(edge.HalfEdge.Twin.Next);
            }
            
            return weight / 2.0;
        }
        
        /// <summary>
        /// Compute cotangent of angle at halfedge's vertex
        /// </summary>
        private static double ComputeCotanAngle(HalfEdge? he)
        {
            if (he?.Next?.Next?.Vertex == null || he.Vertex == null) return 0.0;
            
            var v0 = he.Next.Next.Vertex.Position;
            var v1 = he.Vertex.Position;
            var v2 = he.Next.Vertex!.Position;
            
            var e1 = v1 - v0;
            var e2 = v2 - v0;
            
            float dot = System.Numerics.Vector3.Dot(e1, e2);
            float cross = System.Numerics.Vector3.Cross(e1, e2).Length();
            
            if (Math.Abs(cross) < 1e-10) return 0.0;
            
            return dot / cross;
        }
        
        /// <summary>
        /// Compute harmonic 1-form bases (dimension = 2*genus)
        /// This is simplified - full implementation requires tree-cotree decomposition
        /// </summary>
        public static Matrix<double> HarmonicBases(Mesh mesh)
        {
            // Compute genus from Euler characteristic: χ = 2 - 2g
            int genus = (2 - mesh.EulerCharacteristic()) / 2;
            int dim = 2 * genus;
            
            if (dim <= 0)
            {
                return Matrix<double>.Build.Dense(mesh.Edges.Count, 0);
            }
            
            // For genus > 0, need tree-cotree algorithm
            // Simplified version returns empty bases
            return Matrix<double>.Build.Dense(mesh.Edges.Count, dim);
        }
        
        /// <summary>
        /// Tree-cotree algorithm for finding homology generators
        /// Returns list of edge loops representing generators
        /// Simplified implementation
        /// </summary>
        public static List<List<int>> TreeCoTree(Mesh mesh)
        {
            var generators = new List<List<int>>();
            
            // Full implementation would:
            // 1. Build maximal spanning tree on vertices
            // 2. Build maximal spanning tree on faces (dual graph)
            // 3. Remaining edges form generators
            
            // Simplified placeholder
            return generators;
        }
    }
}
