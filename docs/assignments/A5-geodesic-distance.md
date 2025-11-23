# Assignment 5: Geodesic Distance

**Added by Graph Technologies, 2025**

## Objective

Implement the **heat method** for computing geodesic distances on surfaces.

---

## Theory

### Geodesic Distance

Shortest path distance along surface between two points.

**Properties:**
- $d(p,p) = 0$
- $d(p,q) = d(q,p)$ (symmetry)
- $d(p,q) \leq d(p,r) + d(r,q)$ (triangle inequality)
- $d(p,q) \geq |p-q|$ (always ≥ Euclidean)

---

## Heat Method Algorithm

**Key Insight:** Heat naturally flows along geodesics.

### Three Steps

**Step 1: Diffuse heat from source**

Solve heat equation for short time $t$:

$$(M - tL) u = \delta_{\text{source}}$$

where $\delta_{\text{source}}$ is delta function at source vertex.

**Step 2: Compute normalized gradient**

$$X = -\frac{\nabla u}{|\nabla u|}$$

Points in direction of geodesic.

**Step 3: Solve Poisson equation**

$$\Delta \phi = \nabla \cdot X$$

Recovered distance function $\phi \approx d(\cdot, \text{source})$.

---

## Implementation

### Step 1: Heat Flow

```cpp
Eigen::VectorXd solveHeatFlow(const Mesh& mesh, int sourceVertex, double t) {
    Eigen::SparseMatrix<double> L = CotanLaplacian::build(mesh);
    Eigen::SparseMatrix<double> M = CotanLaplacian::buildMassMatrix(mesh);
    
    // System: (M - tL) * u = M * delta
    Eigen::SparseMatrix<double> A = M - t * L;
    
    Eigen::VectorXd rhs = Eigen::VectorXd::Zero(mesh.numVertices());
    rhs(sourceVertex) = 1.0;
    rhs = M * rhs;
    
    // Solve
    Eigen::SparseLU<Eigen::SparseMatrix<double>> solver;
    solver.compute(A);
    return solver.solve(rhs);
}
```

**Timestep selection:**

$$t = h^2$$

where $h$ = mean edge length.

```cpp
double computeTimestep(const Mesh& mesh) {
    double meanLength = 0.0;
    for (const auto& e : mesh.edges) {
        meanLength += e->length();
    }
    meanLength /= mesh.numEdges();
    return meanLength * meanLength;
}
```

---

### Step 2: Integrated Divergence

Compute $\nabla u$ on each face, normalize, then integrate divergence:

```cpp
Eigen::VectorXd computeIntegratedDivergence(const Mesh& mesh, const Eigen::VectorXd& u) {
    Eigen::VectorXd div = Eigen::VectorXd::Zero(mesh.numVertices());
    
    for (const auto& f : mesh.faces) {
        auto verts = f->vertices();
        
        // Compute gradient of u
        Eigen::Vector3d grad_u = computeGradient(f, u);
        
        // Normalize
        double norm = grad_u.norm();
        if (norm > 1e-10) {
            grad_u /= norm;
        }
        
        // Integrate divergence using cotan weights
        auto halfedges = f->halfedges();
        for (auto he : halfedges) {
            Eigen::Vector3d edge = he->vector();
            double cotWeight = he->edge->cotan();
            
            int i = he->twin->vertex->index;
            int j = he->vertex->index;
            
            double contrib = 0.5 * cotWeight * edge.dot(grad_u);
            div(i) += contrib;
            div(j) -= contrib;
        }
    }
    
    return div;
}
```

---

### Step 3: Distance Recovery

```cpp
Eigen::VectorXd solveDistance(const Mesh& mesh, const Eigen::VectorXd& div) {
    Eigen::SparseMatrix<double> L = CotanLaplacian::build(mesh);
    
    Eigen::SparseLU<Eigen::SparseMatrix<double>> solver;
    solver.compute(L);
    Eigen::VectorXd phi = solver.solve(div);
    
    // Shift so minimum is zero
    phi.array() -= phi.minCoeff();
    
    return phi;
}
```

---

## Complete Pipeline

```cpp
Eigen::VectorXd HeatMethod::compute(const Mesh& mesh, int sourceVertex) {
    // 1. Compute timestep
    double t = computeTimestep(mesh);
    
    // 2. Solve heat flow
    Eigen::VectorXd u = solveHeatFlow(mesh, sourceVertex, t);
    
    // 3. Compute integrated divergence
    Eigen::VectorXd div = computeIntegratedDivergence(mesh, u);
    
    // 4. Recover distance
    Eigen::VectorXd phi = solveDistance(mesh, div);
    
    return phi;
}
```

---

## Testing

### Validation

1. **Distance to self:** $d(v_i, v_i) = 0$
2. **Non-negativity:** $d(v_i, v_j) \geq 0$
3. **Comparison:** $d_{\text{geodesic}} \geq d_{\text{Euclidean}}$
4. **Symmetry:** $d(v_i, v_j) \approx d(v_j, v_i)$

### Test Meshes

**Plane:**
- Geodesic = Euclidean
- Verify exact match

**Sphere:**
- Known geodesic formula
- Compare to great circle distance

**Curved Surface:**
- Geodesic > Euclidean
- Visual check of isolines

---

## Visualization

Color vertices by distance:

```cpp
Eigen::VectorXd dist = HeatMethod::compute(mesh, sourceVertex);
double maxDist = dist.maxCoeff();

for (int i = 0; i < mesh.numVertices(); i++) {
    double t = dist(i) / maxDist;
    Color c = colormap(t); // Blue → Green → Red
    // Apply color to vertex i
}
```

---

## Common Issues

### Issue: Distances not smooth

**Cause:** Timestep too large or too small

**Fix:** Use $t = h^2$ (mean edge length squared)

### Issue: Negative distances

**Cause:** Gradient computation error

**Fix:** Check gradient formula, verify cotan weights

---

## Extensions

1. **Multiple sources:** Voronoi diagrams
2. **Geodesic paths:** Gradient descent on distance field
3. **Cut locus:** Find points equidistant from source
4. **Comparison:** Dijkstra, fast marching methods

---

## References

- Crane et al., "The Heat Method for Distance Computation" (2013)
- Course Notes, Chapter 8
- Assignment page: https://brickisland.net/ddg-web/assignments/assignment5/
