// Edge.cs
// Discrete Differential Geometry - Edge Structure
// Added by Graph Technologies, 2025

using System;
using System.Numerics;

namespace DDGCompanion.Core
{
    public class Edge
    {
        public HalfEdge? HalfEdge { get; set; }
        public int Index { get; set; } = -1;
        
        public Vertex? V0() => HalfEdge?.Twin?.Vertex;
        public Vertex? V1() => HalfEdge?.Vertex;
        
        public bool IsBoundary() => HalfEdge?.Face == null || HalfEdge?.Twin?.Face == null;
        
        public float Length()
        {
            if (HalfEdge?.Vertex == null || HalfEdge?.Twin?.Vertex == null)
                return 0;
            return Vector3.Distance(HalfEdge.Vertex.Position, HalfEdge.Twin.Vertex.Position);
        }
        
        public double Cotan()
        {
            double cotSum = 0.0;
            
            // Cotan from first triangle
            if (HalfEdge?.Face != null)
            {
                var e1 = HalfEdge.Vertex!.Position - HalfEdge.Twin!.Vertex!.Position;
                var e2 = HalfEdge.Next!.Vertex!.Position - HalfEdge.Vertex.Position;
                float denom = e1.Length() * e2.Length();
                if (denom > 1e-12f)
                {
                    float cosAngle = Vector3.Dot(e1, e2) / denom;
                    float sinAngle = Vector3.Cross(e1, e2).Length() / denom;
                    if (Math.Abs(sinAngle) > 1e-12f)
                    {
                        cotSum += cosAngle / sinAngle;
                    }
                }
            }
            
            // Cotan from second triangle
            if (HalfEdge?.Twin?.Face != null)
            {
                var twinHe = HalfEdge.Twin;
                var e1 = twinHe.Vertex!.Position - twinHe.Twin!.Vertex!.Position;
                var e2 = twinHe.Next!.Vertex!.Position - twinHe.Vertex.Position;
                float denom = e1.Length() * e2.Length();
                if (denom > 1e-12f)
                {
                    float cosAngle = Vector3.Dot(e1, e2) / denom;
                    float sinAngle = Vector3.Cross(e1, e2).Length() / denom;
                    if (Math.Abs(sinAngle) > 1e-12f)
                    {
                        cotSum += cosAngle / sinAngle;
                    }
                }
            }
            
            return cotSum / 2.0;
        }
        
        public Vector3 Midpoint()
        {
            if (HalfEdge?.Vertex == null || HalfEdge?.Twin?.Vertex == null)
                return Vector3.Zero;
            return 0.5f * (HalfEdge.Vertex.Position + HalfEdge.Twin.Vertex.Position);
        }
    }
}
