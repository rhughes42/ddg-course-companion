using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using DDGCompanion.Core;

namespace DDGCompanion.Algorithms
{
    public class CotanLaplacian
    {
        public static SparseMatrix Build(Mesh mesh)
        {
            int n = mesh.Vertices.Count;
            var builder = new SparseMatrix(n, n);
            
            foreach (var v in mesh.Vertices)
            {
                int i = v.Index;
                double sumWeights = 0;
                var he = v.HalfEdge;
                if (he == null)
                {
                    continue;
                }

                var visited = new HashSet<int>();
                while (he != null && visited.Add(he.Index))
                {
                    if (he.Vertex == null)
                    {
                        break;
                    }

                    int j = he.Vertex.Index;
                    double weight = ComputeCotanWeight(he);
                    builder[i, j] += weight;
                    sumWeights += weight;

                    if (he.Twin?.Next == null) break;
                    he = he.Twin.Next;
                    if (he == v.HalfEdge) break;
                }
                builder[i, i] = -sumWeights;
            }
            return builder;
        }
        
        public static SparseMatrix BuildMassMatrix(Mesh mesh)
        {
            int n = mesh.Vertices.Count;
            var builder = new SparseMatrix(n, n);
            
            // Barycentric dual area for each vertex
            foreach (var vertex in mesh.Vertices)
            {
                double area = 0.0;
                
                if (vertex.HalfEdge != null)
                {
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
                }
                
                builder[vertex.Index, vertex.Index] = area;
            }
            
            return builder;
        }
        
        private static double ComputeCotanWeight(HalfEdge he)
        {
            if (he.Edge == null) return 0.0;
            var cotan = he.Edge.Cotan();
            return double.IsFinite(cotan) ? cotan : 0.0;
        }
    }
}
