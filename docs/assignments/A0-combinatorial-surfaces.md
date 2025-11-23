# Assignment 0: Combinatorial Surfaces

## Objective

Implement fundamental operations on simplicial complexes and halfedge meshes.

## Tasks

### Part 1: Simplicial Complex Operations

1. **Star**: Given vertex, compute all faces containing it
2. **Closure**: Given simplex, compute all sub-simplices
3. **Link**: Given vertex, compute link (adjacent faces not containing vertex)
4. **Boundary**: Given k-simplex, compute (k-1)-boundary

### Part 2: Halfedge Mesh

5. **Build mesh**: Construct halfedge data structure from V,F arrays
6. **Traversal**: Implement 1-ring neighborhood iteration
7. **Euler characteristic**: Verify $V - E + F = \chi$
8. **Mean valence**: Compute average vertex degree

## Implementation

### Star Operation

```cpp
std::vector<Face*> Vertex::star() const {
    std::vector<Face*> faces;
    if (!halfedge) return faces;
    
    HalfEdge* he = halfedge;
    do {
        if (he->face)
            faces.push_back(he->face);
        he = he->twin->next;
    } while (he != halfedge);
    
    return faces;
}
```

### Link Operation

```cpp
std::vector<Vertex*> Vertex::link() const {
    std::vector<Vertex*> link;
    if (!halfedge) return link;
    
    HalfEdge* he = halfedge;
    do {
        // Vertices in link are opposite to v in each face
        link.push_back(he->next->next->vertex);
        he = he->twin->next;
    } while (he != halfedge);
    
    return link;
}
```

## Testing

### Test Cases

1. **Tetrahedron**: V=4, E=6, F=4, χ=2
2. **Cube**: V=8, E=12, F=6, χ=2
3. **Torus**: V=?, E=?, F=?, χ=0

### Validation

```cpp
void validateMesh(const Mesh& mesh) {
    // Check Euler characteristic
    int chi = mesh.eulerCharacteristic();
    std::cout << "Euler characteristic: " << chi << std::endl;
    
    // Check manifold property
    for (const auto& v : mesh.vertices) {
        auto link = v->link();
        // Link should form a cycle (closed loop)
    }
    
    // Check twin symmetry
    for (const auto& he : mesh.halfedges) {
        assert(he->twin->twin == he.get());
    }
}
```

## Common Issues

1. **Null pointers**: Always check halfedge != nullptr
2. **Infinite loops**: Ensure twin->next cycle terminates
3. **Index mismatch**: Call reindex() after building mesh
4. **Boundary vertices**: Handle cases where face = nullptr

## Grading Criteria

- Correct implementation of all operations
- Proper handling of boundary cases
- Efficient traversal (O(degree) complexity)
- Clean, documented code

## Resources

- [Official Assignment](https://brickisland.net/ddg-web/assignments/assignment0/)
- Implementation: `src/cpp/core/Mesh.cpp`
