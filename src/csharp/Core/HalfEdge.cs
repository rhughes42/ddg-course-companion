namespace DDGCompanion.Core
{
    public class HalfEdge
    {
        public Vertex? Vertex { get; set; }
        public Edge? Edge { get; set; }
        public Face? Face { get; set; }
        public HalfEdge? Next { get; set; }
        public HalfEdge? Twin { get; set; }
        public int Index { get; set; } = -1;
        
        public Vertex? Source() => Twin?.Vertex;
        public Vertex? Target() => Vertex;
        public bool IsBoundary() => Face == null;
    }
}
