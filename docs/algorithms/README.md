# Algorithm Documentation

Comprehensive guide to all implemented DDG algorithms.

> Maturity note: several implementations are currently **Partial** or **Experimental**.  
> See `docs/status/IMPLEMENTATION_AUDIT.md` for current correctness and robustness caveats.

**Added by Graph Technologies, 2025**

---

## Core Algorithms

### 1. Cotan Laplacian

**Files:**
- C++: `src/cpp/algorithms/CotanLaplacian.{h,cpp}`
- C#: `src/csharp/Algorithms/CotanLaplacian.cs`

**Theory:**
The discrete Laplace-Beltrami operator using cotangent weights:

$$L_{ij} = \begin{cases}
\frac{1}{2}(\cot \alpha_{ij} + \cot \beta_{ij}) & \text{if edge } (i,j) \\
-\sum_{k \sim i} L_{ik} & \text{if } i = j \\
0 & \text{otherwise}
\end{cases}$$

**Properties:**
- Symmetric matrix
- Zero row sum
- Positive semi-definite
- Converges to smooth Laplace-Beltrami as mesh refinement → ∞

**Usage:**
```cpp
// C++
auto L = CotanLaplacian::build(mesh);
auto M = CotanLaplacian::buildMassMatrix(mesh);
```

```csharp
// C#
var L = CotanLaplacian.Build(mesh);
var M = CotanLaplacian.BuildMassMatrix(mesh);
```

**Applications:**
- Mesh smoothing
- Poisson equation
- Spectral analysis
- Heat diffusion

---

### 2. Mean Curvature Flow

**Files:**
- C++: `src/cpp/algorithms/MeanCurvatureFlow.{h,cpp}`
- C#: `src/csharp/Algorithms/MeanCurvatureFlow.cs`

**Theory:**
Implicit integration of geometric flow:

$$(M - tL) x^{n+1} = M x^n$$

Unconditionally stable for any timestep $t > 0$.

**Parameters:**
- `timestep`: Integration step size (typical: 0.001 - 0.01)
- `numSteps`: Number of iterations

**Effects:**
- Smooths mesh
- Removes high-frequency noise
- Preserves topology
- Converges to minimal surface

**Usage:**
```cpp
// C++ - Single step
MeanCurvatureFlow::step(mesh, 0.001);

// C++ - Multiple steps
MeanCurvatureFlow::flow(mesh, 0.001, 100);
```

```csharp
// C# - Single step
MeanCurvatureFlow.Step(mesh, 0.001);

// C# - Multiple steps
MeanCurvatureFlow.Flow(mesh, 0.001, 100);
```

---

### 3. Discrete Gaussian Curvature

**Files:**
- C++: `src/cpp/algorithms/DiscreteGaussianCurvature.{h,cpp}`
- C#: `src/csharp/Algorithms/DiscreteGaussianCurvature.cs`

**Theory:**
Angle defect formula:

$$K_i = 2\pi - \sum_{j \in N(i)} \theta_{ij}$$

where $\theta_{ij}$ are angles at vertex $i$ in incident faces.

**Gauss-Bonnet Theorem:**

$$\sum_{i=1}^{|V|} K_i = 2\pi \chi(M)$$

Verifies: total discrete curvature equals $2\pi \times$ Euler characteristic.

**Usage:**
```cpp
// C++
Eigen::VectorXd K = DiscreteGaussianCurvature::compute(mesh);
double totalK = DiscreteGaussianCurvature::totalCurvature(mesh);
```

```csharp
// C#
var K = DiscreteGaussianCurvature.Compute(mesh);
double totalK = DiscreteGaussianCurvature.TotalCurvature(mesh);
```

---

### 4. Conformal Parameterization

**Files:**
- C++: `src/cpp/algorithms/ConformalParameterization.{h,cpp}`
- C#: `src/csharp/Algorithms/ConformalParameterization.cs`

**Methods:**

#### a) Spectral Conformal
Uses eigenvectors of Laplacian for angle-preserving mapping.

**Algorithm:**
1. Solve: $L\phi = \lambda M\phi$
2. Use 2nd and 3rd eigenvectors as $(u,v)$ coordinates

**Usage:**
```cpp
Eigen::MatrixXd uv = ConformalParameterization::spectral(mesh);
```

#### b) Boundary Circle
Maps boundary to unit circle, harmonic interior.

**Algorithm:**
1. Find boundary vertices
2. Map boundary to circle: $(\cos(2\pi i/n), \sin(2\pi i/n))$
3. Solve $\Delta u = 0$ for interior

**Usage:**
```cpp
Eigen::MatrixXd uv = ConformalParameterization::boundaryCircle(mesh);
```

#### c) LSCM (Least Squares Conformal Maps)
Free boundary with soft constraints.

**Usage:**
```cpp
std::vector<int> fixedVerts = {0, 1};
Eigen::MatrixXd fixedPos(2, 2);
fixedPos << 0, 0, 1, 0;
Eigen::MatrixXd uv = ConformalParameterization::lscm(mesh, fixedVerts, fixedPos);
```

---

### 5. Heat Method (Geodesic Distance)

**Files:**
- C++: `src/cpp/algorithms/HeatMethod.{h,cpp}`
- C#: `src/csharp/Algorithms/HeatMethod.cs`

**Theory:**
Compute geodesic distance via short-time heat diffusion.

**Algorithm:**
1. **Heat diffusion**: $(M - tL)u = \delta_{\text{source}}$
2. **Normalize gradient**: $X = -\nabla u / |\nabla u|$
3. **Solve Poisson**: $\Delta \phi = \nabla \cdot X$

**Timestep:**
$$t = h^2$$
where $h$ = mean edge length.

**Usage:**
```cpp
// C++ - Single source
Eigen::VectorXd dist = HeatMethod::compute(mesh, sourceVertex);

// C++ - Multiple sources
std::vector<int> sources = {0, 5, 10};
Eigen::VectorXd dist = HeatMethod::compute(mesh, sources);
```

```csharp
// C# - Single source
var dist = HeatMethod.Compute(mesh, sourceVertex);

// C# - Multiple sources
var dist = HeatMethod.Compute(mesh, 0, 5, 10);
```

**Properties:**
- $O(n)$ complexity (linear solve)
- Much faster than Dijkstra for dense sampling
- Accurate for smooth geodesics
- Handles multiple sources efficiently

---

### 6. Hodge Decomposition

**Files:**
- C++: `src/cpp/algorithms/HodgeDecomposition.{h,cpp}`
- C#: `src/csharp/Algorithms/HodgeDecomposition.cs`

**Theory:**
Decompose 1-form into orthogonal components:

$$\omega = d\alpha + \delta\beta + \gamma$$

where:
- $d\alpha$: exact (curl-free)
- $\delta\beta$: coexact (divergence-free)
- $\gamma$: harmonic (both curl-free and divergence-free)

**Algorithm:**
1. Build exterior derivatives $d_0$, $d_1$
2. Build Hodge stars $\star_0$, $\star_1$, $\star_2$
3. Compute codifferential: $\delta = \star d \star$
4. Solve least squares: $d_0\alpha \approx \omega$
5. Solve: $\delta_1\beta \approx \omega - d_0\alpha$
6. Harmonic: $\gamma = \omega - d_0\alpha - \delta_1\beta$

**Usage:**
```cpp
// C++
Eigen::VectorXd omega = // input 1-form
VectorFieldComponents comp = HodgeDecomposition::decompose(mesh, omega);

// Access components
Eigen::VectorXd exact = comp.exact;
Eigen::VectorXd coexact = comp.coexact;
Eigen::VectorXd harmonic = comp.harmonic;
```

```csharp
// C#
var omega = Vector<double>.Build.Dense(mesh.Edges.Count);
// ... initialize omega ...
var components = HodgeDecomposition.Decompose(mesh, omega);

// Access components
var exact = components.Exact;
var coexact = components.Coexact;
var harmonic = components.Harmonic;
```

**Applications:**
- Vector field design
- Fluid simulation
- Tangent direction fields
- Surface remeshing

---

## Algorithm Complexity

| Algorithm | Time Complexity | Space | Notes |
|-----------|----------------|-------|-------|
| Cotan Laplacian | $O(V)$ | $O(V)$ | Sparse matrix, $O(V)$ non-zeros |
| Mean Curvature Flow | $O(V \cdot k)$ | $O(V)$ | $k$ = iterations, sparse solve |
| Gaussian Curvature | $O(V)$ | $O(V)$ | Linear pass over vertices |
| Heat Method | $O(V)$ | $O(V)$ | 3 sparse linear solves |
| Conformal (Spectral) | $O(V^3)$ | $O(V^2)$ | Eigendecomposition |
| Hodge Decomposition | $O(V)$ | $O(V)$ | Sparse least squares |

---

## Implementation Notes

### Numerical Stability

**Issue:** Obtuse triangles produce negative cotangent weights.

**Solutions:**
1. **Clamp weights:** `cot = max(cot, epsilon)`
2. **Intrinsic Delaunay:** Flip edges to remove obtuse angles
3. **Alternative weights:** Mean value coordinates

### Boundary Handling

**Interior vertices:** Full Laplacian stencil

**Boundary vertices:**
- **Dirichlet:** Fix value, remove from system
- **Neumann:** Natural boundary conditions (zero normal derivative)

### Solver Selection

**C++ (Eigen):**
- `SparseLU`: General, stable
- `SimplicialLDLT`: Faster for SPD matrices
- `ConjugateGradient`: Iterative, memory efficient

**C# (MathNet):**
- `DenseMatrix.Solve()`: Small problems
- Iterative solvers for large sparse systems

---

## Testing Guidelines

### Validation Tests

**Laplacian:**
- Zero row sum
- Symmetry
- Positive semi-definite

**Curvature:**
- Gauss-Bonnet: $\sum K_i = 2\pi\chi$
- Known shapes (sphere: $K = 1/r^2$ everywhere)

**Parameterization:**
- Bijectivity (no flipped triangles)
- Conformal distortion (angle preservation)

**Heat Method:**
- Distance from vertex to itself = 0
- Non-negativity
- Triangle inequality

### Test Meshes

**Simple:**
- Tetrahedron: V=4, E=6, F=4, χ=2
- Cube: V=8, E=12, F=6, χ=2
- Octahedron: V=6, E=12, F=8, χ=2

**Complex:**
- Sphere (subdivided icosahedron)
- Torus: χ=0, genus=1
- Bunny, Armadillo (Stanford models)

---

## Performance Optimization

### Memory

- Use **sparse matrices** (Eigen::SparseMatrix, MathNet sparse types)
- Preallocate triplet lists with estimated size
- Reuse factorizations when possible

### Computation

**Profiling hotspots:**
1. Matrix assembly (triplet insertion)
2. Linear system solve
3. Mesh traversal

**Optimizations:**
- Cache cotan weights if mesh doesn't change
- Use iterative solvers for large systems
- Parallelize independent face operations
- Consider GPU for very large meshes (CUDA, OpenCL)

---

## Common Issues

### Problem: Laplacian produces NaN

**Causes:**
- Degenerate triangles (zero area)
- Colinear vertices
- Duplicate vertices

**Solution:**
```cpp
// Check for degenerate faces
for (const auto& f : mesh.faces) {
    if (f->area() < 1e-10) {
        std::cerr << "Degenerate face: " << f->index << std::endl;
    }
}
```

### Problem: Mean curvature flow explodes

**Cause:** Timestep too large

**Solution:** Reduce timestep or use smaller steps

### Problem: Parameterization has flipped triangles

**Cause:** Boundary constraints too restrictive

**Solution:** Use free boundary method (spectral or LSCM)

---

## Extension Ideas

### Advanced Algorithms

- Anisotropic diffusion
- Bilateral mesh filtering
- Poisson surface reconstruction
- As-rigid-as-possible deformation
- Quadrilateral remeshing

### Optimizations

- Multigrid solvers
- GPU acceleration
- Parallel matrix assembly
- Cache-friendly data structures

### Integrations

- Rhino3D/Grasshopper plugin
- Blender addon
- Unity/Unreal game engine
- Web (via WebAssembly)

---

## References

1. **Crane et al.** "Discrete Differential Geometry: An Applied Introduction" (Course Notes)
2. **Meyer et al.** "Discrete Differential-Geometry Operators for Triangulated 2-Manifolds" (2003)
3. **Desbrun et al.** "Implicit Fairing of Irregular Meshes using Diffusion and Curvature Flow" (1999)
4. **Crane et al.** "The Heat Method for Distance Computation" (2013)
5. **Lévy et al.** "Least Squares Conformal Maps" (2002)
