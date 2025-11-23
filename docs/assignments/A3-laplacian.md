# Assignment 3: The Laplacian

## Objective

Implement the cotan-Laplace operator and apply it to geometry processing tasks.

## Tasks

### Part 1: Cotan Laplacian

1. **Build Laplacian matrix L**: Implement cotan formula
2. **Build mass matrix M**: Compute vertex areas
3. **Solve Poisson equation**: Δu = f

### Part 2: Applications

4. **Mesh smoothing**: Implicit mean curvature flow
5. **Scalar field smoothing**: Diffuse vertex function
6. **Harmonic interpolation**: Interpolate boundary values

## Theory

### Cotan Laplacian Formula

For interior vertex $i$:

$$L_{ij} = \begin{cases}
\frac{1}{2}(\cot \alpha_{ij} + \cot \beta_{ij}) & \text{if } (i,j) \text{ edge} \\
-\sum_{j \sim i} L_{ij} & \text{if } i = j \\
0 & \text{otherwise}
\end{cases}$$

where $\alpha_{ij}, \beta_{ij}$ are opposite angles.

### Mean Curvature Flow

Implicit integration:

$$(M - tL) x^{n+1} = M x^n$$

Stable for any timestep $t > 0$.

## Implementation

### Cotan Weight

```cpp
double Edge::cotan() const {
    double cotSum = 0.0;
    
    // Cotan from first triangle
    if (halfedge->face) {
        Eigen::Vector3d e1 = halfedge->vector();
        Eigen::Vector3d e2 = halfedge->next->vector();
        double cosAngle = e1.dot(e2) / (e1.norm() * e2.norm());
        double sinAngle = e1.cross(e2).norm() / (e1.norm() * e2.norm());
        cotSum += cosAngle / sinAngle;
    }
    
    // Cotan from second triangle (twin side)
    if (halfedge->twin->face) {
        Eigen::Vector3d e1 = halfedge->twin->vector();
        Eigen::Vector3d e2 = halfedge->twin->next->vector();
        double cosAngle = e1.dot(e2) / (e1.norm() * e2.norm());
        double sinAngle = e1.cross(e2).norm() / (e1.norm() * e2.norm());
        cotSum += cosAngle / sinAngle;
    }
    
    return 0.5 * cotSum;
}
```

### Assembling Laplacian

```cpp
Eigen::SparseMatrix<double> CotanLaplacian::build(const Mesh& mesh) {
    int n = mesh.numVertices();
    Eigen::SparseMatrix<double> L(n, n);
    std::vector<Eigen::Triplet<double>> triplets;
    
    for (const auto& v : mesh.vertices) {
        double sumWeights = 0.0;
        
        HalfEdge* he = v->halfedge;
        do {
            Vertex* neighbor = he->next->vertex;
            double weight = he->edge->cotan();
            
            triplets.push_back({v->index, neighbor->index, weight});
            sumWeights += weight;
            
            he = he->twin->next;
        } while (he != v->halfedge);
        
        triplets.push_back({v->index, v->index, -sumWeights});
    }
    
    L.setFromTriplets(triplets.begin(), triplets.end());
    return L;
}
```

### Mean Curvature Flow

```cpp
void MeanCurvatureFlow::step(Mesh& mesh, double timestep) {
    auto L = CotanLaplacian::build(mesh);
    auto M = CotanLaplacian::buildMassMatrix(mesh);
    
    Eigen::SparseMatrix<double> A = M - timestep * L;
    Eigen::MatrixXd X = mesh.vertexPositions();
    Eigen::MatrixXd b = M * X;
    
    Eigen::SparseLU<Eigen::SparseMatrix<double>> solver;
    solver.compute(A);
    Eigen::MatrixXd X_new = solver.solve(b);
    
    mesh.setVertexPositions(X_new);
}
```

## Testing

### Validation

1. **Symmetry**: L should be symmetric
2. **Zero row sum**: Each row should sum to zero
3. **Positive semi-definite**: All eigenvalues ≥ 0
4. **Convergence**: MCF should smooth mesh

### Test Meshes

- Noisy sphere: Should converge to smooth sphere
- Cube: Should converge to rounded cube
- Torus: Topology preserved, smoothed

## Common Issues

1. **Negative cotangents**: Obtuse triangles give negative weights
2. **Numerical instability**: Use SparseLU instead of SimplicialLDLT
3. **Boundary handling**: Set boundary vertices as constraints
4. **Degenerate triangles**: Check for zero area

## Extensions

- Anisotropic diffusion
- Bilateral filtering
- Feature-preserving smoothing

## Resources

- [Official Assignment](https://brickisland.net/ddg-web/assignments/assignment3/)
- Implementation: `src/cpp/algorithms/CotanLaplacian.cpp`
- Implementation: `src/cpp/algorithms/MeanCurvatureFlow.cpp`
