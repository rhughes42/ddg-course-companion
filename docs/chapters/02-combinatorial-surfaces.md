# Chapter 2: Combinatorial Surfaces

## Abstract Simplicial Complex

### Definition

An **abstract simplicial complex** $K$ is a collection of finite sets closed under subsets:
- If $\sigma \in K$ and $\tau \subseteq \sigma$, then $\tau \in K$

### Elements

- **0-simplex**: Vertex $\{v\}$
- **1-simplex**: Edge $\{v_0, v_1\}$
- **2-simplex**: Face $\{v_0, v_1, v_2\}$
- **k-simplex**: Set of $(k+1)$ vertices

## Simplicial Operations

### Star

The **star** of simplex $\sigma$:
$$\text{St}(\sigma) = \{ \tau \in K : \sigma \subseteq \tau \}$$

All simplices containing $\sigma$.

### Closure

The **closure** of simplex $\sigma$:
$$\text{Cl}(\sigma) = \{ \tau \in K : \tau \subseteq \sigma \}$$

All faces of $\sigma$.

### Link

The **link** of simplex $\sigma$:
$$\text{Lk}(\sigma) = \text{Cl}(\text{St}(\sigma)) \setminus \text{St}(\text{Cl}(\sigma))$$

Simplices adjacent to but not containing $\sigma$.

## Manifolds

A simplicial complex is a **manifold** if:
- Link of every vertex is either:
  - A closed loop (interior vertex)
  - A path (boundary vertex)

## Euler's Formula

$$V - E + F = \chi(M)$$

Where:
- $V$ = number of vertices
- $E$ = number of edges
- $F$ = number of faces
- $\chi$ = Euler characteristic (topological invariant)

For closed surfaces: $\chi = 2 - 2g$ where $g$ is genus.

## Halfedge Mesh

### Data Structure

Optimized for **oriented 2-manifold triangulated surfaces**.

**Elements:**
- `Vertex`: Position, outgoing halfedge pointer
- `HalfEdge`: Target vertex, face, next, twin, edge
- `Edge`: One halfedge pointer
- `Face`: One halfedge pointer

### Traversal

```cpp
// 1-ring neighborhood of vertex v
HalfEdge* he = v->halfedge;
do {
    Vertex* neighbor = he->next->vertex;
    // Process neighbor
    he = he->twin->next;
} while (he != v->halfedge);
```

## Implementation

- C++: `src/cpp/core/Mesh.h`, `src/cpp/core/HalfEdge.h`
- C#: `src/csharp/Core/Mesh.cs`

## Exercises

1. Verify Euler's formula for the 5 Platonic solids
2. Implement star, closure, link operations
3. Check manifold condition for given mesh
4. Compute mean vertex valence

## References

- Course notes: Chapter 2
- Assignment A0: Combinatorial Surfaces
