using System;
using System.Collections.Generic;
using System.Numerics;

namespace DDGCompanion.Core
{
    public class Vertex
    {
        public Vector3 Position { get; set; }
        public HalfEdge? HalfEdge { get; set; }
        public int Index { get; set; } = -1;
        
        public Vertex(Vector3 position)
        {
            Position = position;
        }
        
        public List<Vertex> Star()
        {
            var neighbors = new List<Vertex>();
            if (HalfEdge == null) return neighbors;
            
            var he = HalfEdge;
            do
            {
                neighbors.Add(he.Next!.Vertex!);
                he = he.Twin!.Next!;
            } while (he != HalfEdge);
            
            return neighbors;
        }
        
        public int Degree()
        {
            if (HalfEdge == null) return 0;
            int count = 0;
            var he = HalfEdge;
            do
            {
                count++;
                he = he.Twin!.Next!;
            } while (he != HalfEdge);
            return count;
        }
        
        public bool IsBoundary()
        {
            if (HalfEdge == null) return true;
            var he = HalfEdge;
            do
            {
                if (he.Face == null) return true;
                he = he.Twin!.Next!;
            } while (he != HalfEdge);
            return false;
        }
    }
}
