# Assignment 2: Curvature

**Added by Graph Technologies, 2025**

## Objective

Implement discrete Gaussian curvature and mean curvature operators for triangulated surfaces. Verify the discrete Gauss-Bonnet theorem.

---

## Tasks

### Task 1: Vertex Normal Computation

Implement vertex normal using **vector area method**.

**Formula:**

$$N_i = \frac{1}{2} \sum_{jk} (p_j - p_i) \times (p_k - p_i)$$

where sum is over edges in star of vertex $i$.

**Implementation (C++):**
```cpp
Eigen::Vector3d Vertex::normal() const {
    Eigen::Vector3d N = Eigen::Vector3d::Zero();
    
    HalfEdge* he = halfedge;
    do {
        if (he->face) {
            Eigen::Vector3d e1 = he->twin->vertex->position - position;
            Eigen::Vector3d e2 = he->next->vertex->position - position;
            N += e1.cross(e2);
        }
        he = he->twin->next;
    } while (he != halfedge);
    
    return N.normalized();
}
```

**Testing:**
- Sphere: normals should point outward radially
- Flat plane: all normals should be (0,0,1)

---

### Task 2: Discrete Gaussian Curvature

Implement **angle defect** formula.

**Formula:**

$$K_i = 2\pi - \sum_{j \in \text{faces}(i)} \theta_{ij}$$

**Steps:**

1. Loop over faces incident to vertex
2. Compute angle at vertex in each face
3. Sum angles
4. Compute defect: $2\pi -$ sum

**Implementation Guide:**

```cpp
double Vertex::gaussianCurvature() const {
    if (isBoundary()) return 0.0;
    
    double angleSum = 0.0;
    
    // Traverse faces around vertex
    HalfEdge* he = halfedge;
    do {
        if (he->face) {
            // Compute angle at this vertex
            Eigen::Vector3d e1 = (he->twin->vertex->position - position).normalized();
            Eigen::Vector3d e2 = (he->next->vertex->position - position).normalized();
            
            double cosAngle = std::clamp(e1.dot(e2), -1.0, 1.0);
            double angle = std::acos(cosAngle);
            angleSum += angle;
        }
        he = he->twin->next;
    } while (he != halfedge);
    
    return 2.0 * M_PI - angleSum;
}
```

**Edge Cases:**
- Boundary vertices: Convention is $K = 0$ or handle separately
- Degenerate faces: Check for zero area

---

### Task 3: Verify Gauss-Bonnet

Compute total curvature and compare to $2\pi\chi(M)$.

**Algorithm:**

```cpp
double totalCurvature = 0.0;
for (const auto& v : mesh.vertices) {
    totalCurvature += v->gaussianCurvature();
}

int chi = mesh.eulerCharacteristic();
double expected = 2.0 * M_PI * chi;
double error = std::abs(totalCurvature - expected);

std::cout << "Total K: " << totalCurvature << std::endl;
std::cout << "Expected: " << expected << std::endl;
std::cout << "Error: " << error << std::endl;
```

**Test Meshes:**

| Mesh | $\chi$ | Expected | Typical Error |
|------|--------|----------|---------------|
| Tetrahedron | 2 | $4\pi$ | < 0.01 |
| Cube | 2 | $4\pi$ | < 0.01 |
| Sphere (subdiv) | 2 | $4\pi$ | < 0.001 |
| Torus | 0 | $0$ | < 0.01 |

---

### Task 4: Mean Curvature (Optional)

Implement mean curvature from area gradient.

**Formula:**

$$H_i = \frac{|\nabla A_i|}{2A_i}$$

**Note:** Sign of $H$ requires orienting normal correctly.

---

## Implementation Checklist

- [ ] `Vertex::normal()` - Vector area method
- [ ] `Vertex::gaussianCurvature()` - Angle defect
- [ ] `DiscreteGaussianCurvature::compute()` - All vertices
- [ ] `DiscreteGaussianCurvature::totalCurvature()` - Sum over mesh
- [ ] Gauss-Bonnet verification function
- [ ] Handle boundary vertices
- [ ] Handle degenerate cases
- [ ] Unit tests
- [ ] Visualization (curvature colormap)

---

## Testing Strategy

### Unit Tests

1. **Platonic solids:** Known uniform curvature
2. **Gauss-Bonnet:** Verify theorem holds
3. **Boundary handling:** Ensure no crashes

### Visual Tests

1. **Sphere:** Positive curvature everywhere (red)
2. **Saddle surface:** Mixed positive/negative (red/blue)
3. **Flat regions:** Near-zero curvature (white)

---

## Common Issues

### Issue: Curvature is NaN

**Cause:** `acos()` domain error from floating point

**Fix:**
```cpp
double cosAngle = std::clamp(e1.dot(e2), -1.0, 1.0);
```

### Issue: Gauss-Bonnet fails

**Causes:**
- Non-manifold mesh
- Incorrect halfedge connectivity
- Degenerate faces included

**Debug:**
```cpp
std::cout << "Chi: " << mesh.eulerCharacteristic() << std::endl;
for (const auto& v : mesh.vertices) {
    if (std::isnan(v->gaussianCurvature())) {
        std::cout << "NaN at vertex " << v->index << std::endl;
    }
}
```

---

## Extensions

1. **Principal curvatures** $\kappa_1$, $\kappa_2$
2. **Shape operator** matrix
3. **Mean curvature normal** $H \cdot N$
4. **Curvature tensor** visualization
5. **Curvature-based segmentation**

---

## References

- Course Notes, Chapter 5
- Meyer et al., "Discrete Differential-Geometry Operators" (2003)
- Assignment page: https://brickisland.net/ddg-web/assignments/assignment2/
