# Assignment Guides

Detailed guides for all DDG programming assignments.

## Mandatory Assignments

- [A0: Combinatorial Surfaces](A0-combinatorial-surfaces.md)
- [A1: Discrete Exterior Calculus](A1-exterior-calculus.md)

## Optional Assignments (Pick 3)

- [A2: Curvature](A2-curvature.md)
- [A3: The Laplacian](A3-laplacian.md)
- [A4: Conformal Parameterization](A4-parameterization.md)
- [A5: Geodesic Distance](A5-geodesic.md)

## Extra Credit

- [A6: Vector Field Decomposition](A6-vector-fields.md)

## Implementation Tips

1. **Start with data structures**: Ensure halfedge mesh works correctly
2. **Test incrementally**: Verify each function before moving on
3. **Visualize results**: Use MeshLab or similar to check output
4. **Handle edge cases**: Boundary vertices, degenerate triangles
5. **Numerical stability**: Watch for division by zero, NaN propagation

## Debugging Strategies

### Mesh Connectivity Issues

```cpp
// Verify halfedge structure
void validateMesh(const Mesh& mesh) {
    for (const auto& he : mesh.halfedges) {
        assert(he->twin->twin == he.get());
        assert(he->next->next->next == he.get());
        assert(he->edge == he->twin->edge);
    }
}
```

### Matrix Assembly

```cpp
// Check symmetry of Laplacian
bool isSymmetric(const Eigen::SparseMatrix<double>& L) {
    for (int k = 0; k < L.outerSize(); ++k) {
        for (Eigen::SparseMatrix<double>::InnerIterator it(L,k); it; ++it) {
            if (std::abs(it.value() - L.coeff(it.col(), it.row())) > 1e-10)
                return false;
        }
    }
    return true;
}
```

## Resources

- [Course Framework (C++)](https://github.com/dgpdec/course)
- [libigl](https://libigl.github.io/) - Geometry processing library
- [Eigen](https://eigen.tuxfamily.org/) - Linear algebra library
