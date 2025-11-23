# Assignment 4: Conformal Parameterization

**Added by Graph Technologies, 2025**

## Objective

Implement spectral conformal parameterization to flatten surfaces to 2D while preserving angles.

---

## Theory

### Conformal Maps

A map $f: M \to \mathbb{R}^2$ is **conformal** if it preserves angles:

$$\frac{\partial u}{\partial x} = \frac{\partial v}{\partial y}, \quad \frac{\partial u}{\partial y} = -\frac{\partial v}{\partial x}$$

(Cauchy-Riemann equations)

### Dirichlet Energy

$$E_D(f) = \int_M |\nabla f|^2 dA$$

Minimizing $E_D$ subject to constraints yields conformal maps.

---

## Tasks

### Task 1: Build Cotan Laplacian

Reuse from Assignment 3:

```cpp
Eigen::SparseMatrix<double> L = CotanLaplacian::build(mesh);
Eigen::SparseMatrix<double> M = CotanLaplacian::buildMassMatrix(mesh);
```

---

### Task 2: Solve Eigenvalue Problem

Compute first few eigenpairs of generalized problem:

$$L \phi = \lambda M \phi$$

**Using Eigen:**

```cpp
#include <Eigen/Eigenvalues>

// Convert to dense (for small meshes)
Eigen::MatrixXd L_dense = Eigen::MatrixXd(L);
Eigen::MatrixXd M_dense = Eigen::MatrixXd(M);

// Solve
Eigen::GeneralizedSelfAdjointEigenSolver<Eigen::MatrixXd> solver(L_dense, M_dense);

// Extract eigenvectors 1 and 2 (skip 0, which is constant)
Eigen::VectorXd u = solver.eigenvectors().col(1);
Eigen::VectorXd v = solver.eigenvectors().col(2);
```

**For large meshes:** Use sparse eigenvalue solver (Spectra library).

---

### Task 3: Construct UV Coordinates

```cpp
Eigen::MatrixXd uv(mesh.numVertices(), 2);
uv.col(0) = u;
uv.col(1) = v;
```

**Normalization (optional):**
- Scale to $[0,1] \times [0,1]$
- Center at origin

---

### Task 4: Measure Distortion

Compute **Dirichlet energy** of parameterization:

```cpp
double energy = 0.0;

for (const auto& f : mesh.faces) {
    auto verts = f->vertices();
    
    // 3D area
    Eigen::Vector3d e1 = verts[1]->position - verts[0]->position;
    Eigen::Vector3d e2 = verts[2]->position - verts[0]->position;
    double area3D = 0.5 * e1.cross(e2).norm();
    
    // UV gradient
    Eigen::Vector2d uv0 = uv.row(verts[0]->index);
    Eigen::Vector2d uv1 = uv.row(verts[1]->index);
    Eigen::Vector2d uv2 = uv.row(verts[2]->index);
    
    Eigen::Vector2d g1 = uv1 - uv0;
    Eigen::Vector2d g2 = uv2 - uv0;
    
    double gradNorm = g1.squaredNorm() + g2.squaredNorm();
    energy += area3D * gradNorm;
}

std::cout << "Dirichlet energy: " << energy << std::endl;
```

Lower energy → less distortion.

---

## Advanced: Boundary Constraints

For surfaces with boundary, map boundary to circle:

```cpp
// Find boundary vertices
std::vector<int> boundaryVerts;
for (const auto& v : mesh.vertices) {
    if (v->isBoundary()) {
        boundaryVerts.push_back(v->index);
    }
}

// Map to unit circle
for (size_t i = 0; i < boundaryVerts.size(); i++) {
    double angle = 2.0 * M_PI * i / boundaryVerts.size();
    int vIdx = boundaryVerts[i];
    uv(vIdx, 0) = std::cos(angle);
    uv(vIdx, 1) = std::sin(angle);
}

// Solve Laplace equation for interior
// (Modify system to fix boundary values)
```

---

## Implementation Checklist

- [ ] Build Laplacian and mass matrix
- [ ] Solve generalized eigenvalue problem
- [ ] Extract 2nd and 3rd eigenvectors
- [ ] Construct UV coordinates
- [ ] Measure Dirichlet energy
- [ ] Handle boundary constraints (optional)
- [ ] Visualize parameterization
- [ ] Check for triangle flips

---

## Testing

### Test Cases

1. **Sphere:**
   - Flattening should show minimal distortion
   - No triangle flips
   - Roughly circular boundary

2. **Cylinder:**
   - Should unroll to rectangle
   - Straight boundary lines

3. **Disk:**
   - Boundary → circle
   - Interior smoothly distributed

### Validation

**Check for flipped triangles:**
```cpp
for (const auto& f : mesh.faces) {
    auto verts = f->vertices();
    Eigen::Vector2d uv0 = uv.row(verts[0]->index);
    Eigen::Vector2d uv1 = uv.row(verts[1]->index);
    Eigen::Vector2d uv2 = uv.row(verts[2]->index);
    
    double signedArea = 0.5 * ((uv1.x() - uv0.x()) * (uv2.y() - uv0.y()) -
                               (uv2.x() - uv0.x()) * (uv1.y() - uv0.y()));
    
    if (signedArea < 0) {
        std::cout << "Flipped triangle: " << f->index << std::endl;
    }
}
```

---

## Common Issues

### Issue: UV coordinates all zero

**Cause:** Used 1st eigenvector (constant function)

**Fix:** Use 2nd and 3rd eigenvectors

### Issue: Eigenvalue solver fails

**Cause:** Matrix not positive definite

**Fix:** Check Laplacian construction, ensure no isolated vertices

---

## Extensions

1. **LSCM** (Least Squares Conformal Maps)
2. **Free boundary** parameterization
3. **Seamless parameterization** across chart boundaries
4. **Texture mapping** visualization

---

## Resources

- Course Notes, Chapter 7
- Lévy et al., "Least Squares Conformal Maps" (2002)
- Assignment page: https://brickisland.net/ddg-web/assignments/assignment4/
