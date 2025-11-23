# Assignment 6: Vector Fields on Surfaces (Extra Credit)

**Added by Graph Technologies, 2025**

## Objective

Implement Hodge decomposition for discrete vector fields. Design smooth tangent direction fields with prescribed singularities.

---

## Theory

### Hodge Decomposition

Every vector field (1-form) $\omega$ uniquely decomposes:

$$\omega = d\alpha + \delta\beta + \gamma$$

where:
- $d\alpha$: **exact** (curl-free, $d\alpha = 0$ in smooth case means $\nabla \times \alpha = 0$)
- $\delta\beta$: **coexact** (divergence-free)
- $\gamma$: **harmonic** (both curl-free and div-free)

**Orthogonality:** Components are $L^2$-orthogonal.

---

## Discrete Operators

### Exterior Derivative

**$d_0$: 0-forms → 1-forms (vertices → edges)**

$$(d_0 \alpha)_{ij} = \alpha_j - \alpha_i$$

**Matrix form:** Incidence matrix

```cpp
Eigen::SparseMatrix<double> d0(numEdges, numVertices);
for (const auto& e : mesh.edges) {
    int v0 = e->halfedge->twin->vertex->index;
    int v1 = e->halfedge->vertex->index;
    d0.insert(e->index, v0, -1.0);
    d0.insert(e->index, v1,  1.0);
}
```

**$d_1$: 1-forms → 2-forms (edges → faces)**

Sum of edge values around face:

$$(d_1 \omega)_f = \sum_{e \in \partial f} \sigma_e \omega_e$$

where $\sigma_e = \pm 1$ depends on orientation.

---

### Hodge Star

**$\star_0$: 0-forms → 2-forms**

Diagonal matrix with vertex areas:

$$\star_0 = \text{diag}(A_1, A_2, \ldots, A_n)$$

**$\star_1$: 1-forms → 1-forms**

Ratio of dual/primal edge lengths (simplified: identity).

**$\star_2$: 2-forms → 0-forms**

Inverse face areas.

---

### Codifferential

$$\delta = \star d \star$$

**For 1-forms:**

$$\delta_1 = \star_0 d_1^T \star_2$$

---

## Implementation

### Task 1: Build DEC Operators

```cpp
class DECOperators {
public:
    Eigen::SparseMatrix<double> d0, d1;
    Eigen::SparseMatrix<double> star0, star1, star2;
    Eigen::SparseMatrix<double> delta0, delta1;
    
    void build(const Mesh& mesh) {
        d0 = buildD0(mesh);
        d1 = buildD1(mesh);
        star0 = buildStar0(mesh);
        star1 = buildStar1(mesh);
        star2 = buildStar2(mesh);
        delta1 = star0 * d1.transpose() * star2;
    }
};
```

---

### Task 2: Hodge Decomposition

```cpp
VectorFieldComponents HodgeDecomposition::decompose(
    const Mesh& mesh, 
    const Eigen::VectorXd& omega
) {
    DECOperators ops;
    ops.build(mesh);
    
    // Solve d0*alpha = omega (least squares)
    Eigen::SparseQR<Eigen::SparseMatrix<double>> qr;
    qr.compute(ops.d0);
    Eigen::VectorXd alpha = qr.solve(omega);
    Eigen::VectorXd exact = ops.d0 * alpha;
    
    // Solve delta1*beta = omega - exact
    Eigen::VectorXd residual = omega - exact;
    qr.compute(ops.delta1);
    Eigen::VectorXd beta = qr.solve(residual);
    Eigen::VectorXd coexact = ops.delta1 * beta;
    
    // Harmonic component
    Eigen::VectorXd harmonic = omega - exact - coexact;
    
    return {exact, coexact, harmonic};
}
```

---

### Task 3: Vector Field Design

Design smooth tangent direction field with prescribed singularities.

**Algorithm:**

1. **Input:** Singularity locations and indices
2. **Solve:** Find smoothest field matching constraints
3. **Output:** Direction field on faces

**Constraints:**
- Index sum = Euler characteristic
- Field tangent to surface

---

## Testing

### Test 1: Random Field Decomposition

```cpp
// Generate random 1-form
Eigen::VectorXd omega = Eigen::VectorXd::Random(mesh.numEdges());

// Decompose
auto comp = HodgeDecomposition::decompose(mesh, omega);

// Verify reconstruction
Eigen::VectorXd reconstructed = comp.exact + comp.coexact + comp.harmonic;
double error = (reconstructed - omega).norm();

std::cout << "Reconstruction error: " << error << std::endl;
REQUIRE(error < 1e-6);
```

### Test 2: Known Fields

**Exact field:** $\omega = df$ for scalar $f$

Should have: coexact = 0, harmonic = 0

**Coexact field:** Construct divergence-free field

Should have: exact = 0

---

## Visualization

Draw vector field on surface:

```cpp
void drawVectorField(const Mesh& mesh, const Eigen::VectorXd& field) {
    for (const auto& f : mesh.faces) {
        // Convert edge-based field to face-based direction
        Eigen::Vector3d direction = edgeToFaceVector(f, field);
        
        // Draw arrow at face center
        Eigen::Vector3d center = f->center();
        drawArrow(center, direction);
    }
}
```

---

## Common Issues

### Issue: Reconstruction error large

**Cause:** Operators not built correctly

**Fix:** Verify $d$, $\star$, $\delta$ constructions

### Issue: Harmonic component non-zero on sphere

**Expected:** Sphere has genus 0, so no harmonic forms

**Cause:** Numerical error in least squares

**Tolerance:** Accept $|\gamma| < 10^{-6}$

---

## Extensions

1. **Tree-cotree algorithm** for homology generators
2. **Harmonic bases** computation
3. **Vector field smoothing**
4. **N-RoSy fields** (4-RoSy for quad meshing)
5. **Singularity placement** optimization

---

## References

- Course Notes, Chapter 8
- Fisher et al., "Vector Field Design on Surfaces" (2007)
- Assignment page: https://brickisland.net/ddg-web/assignments/assignment6/
