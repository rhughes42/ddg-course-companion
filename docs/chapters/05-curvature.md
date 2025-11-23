# Chapter 5: Curvature of Discrete Surfaces

**Added by Graph Technologies, 2025**

## Overview

This chapter develops discrete analogues of Gaussian and mean curvature for triangulated surfaces. Multiple equivalent definitions from smooth theory lead to different discrete operators, each with distinct properties.

---

## Key Concepts

### 1. Discrete Normals

**Vector Area Method:**

$$N_i = \frac{1}{2} \sum_{jk \in \text{star}(i)} (p_j - p_i) \times (p_k - p_i)$$

Weighted average of incident face normals.

**Properties:**
- Converges to smooth normal as mesh refines
- Respects face orientation
- Used for rendering and visualization

**Implementation:**
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

---

### 2. Area Gradient (Mean Curvature)

**Cotan Formula:**

$$\nabla A_i = \frac{1}{4} \sum_{j \in N(i)} (\cot \alpha_{ij} + \cot \beta_{ij})(p_j - p_i)$$

where $\alpha_{ij}$, $\beta_{ij}$ are angles opposite edge $(i,j)$.

**Relation to Mean Curvature:**

$$H_i N_i = -\frac{\nabla A_i}{2A_i}$$

**Key Properties:**
- Variational: derivative of vertex area
- Identical to Laplacian with cotan weights
- Used in mean curvature flow

---

### 3. Angle Defect (Gaussian Curvature)

**Definition:**

$$K_i = 2\pi - \sum_{j \in \text{star}(i)} \theta_{ij}$$

**Interpretation:**
- Measures "missing angle" at vertex
- $K_i > 0$: positive curvature (spherical)
- $K_i < 0$: negative curvature (hyperbolic)
- $K_i = 0$: flat (Euclidean)

**Examples:**

| Surface | Vertex Type | Angle Sum | $K$ |
|---------|-------------|-----------|-----|
| Plane | Interior | $2\pi$ | $0$ |
| Sphere | All | $< 2\pi$ | $> 0$ |
| Saddle | All | $> 2\pi$ | $< 0$ |
| Cube corner | Corner | $3 \cdot \pi/2 = 1.5\pi$ | $\pi/2$ |

**Implementation:**
```cpp
double Vertex::angleDefect() const {
    double angleSum = 0.0;
    
    HalfEdge* he = halfedge;
    do {
        if (he->face) {
            Eigen::Vector3d e1 = (he->twin->vertex->position - position).normalized();
            Eigen::Vector3d e2 = (he->next->vertex->position - position).normalized();
            double angle = std::acos(std::clamp(e1.dot(e2), -1.0, 1.0));
            angleSum += angle;
        }
        he = he->twin->next;
    } while (he != halfedge);
    
    return 2.0 * M_PI - angleSum;
}
```

---

## Discrete Gauss-Bonnet Theorem

**Statement:**

$$\sum_{i=1}^{|V|} K_i = 2\pi \chi(M)$$

Total discrete curvature depends only on topology, not geometry.

**Verification:**

```cpp
double totalK = DiscreteGaussianCurvature::totalCurvature(mesh);
double expectedK = 2.0 * M_PI * mesh.eulerCharacteristic();
double error = std::abs(totalK - expectedK);

if (error < 1e-6) {
    std::cout << "Gauss-Bonnet verified!" << std::endl;
}
```

**Test Cases:**

| Mesh | $\chi$ | Expected Total $K$ | Result |
|------|--------|-------------------|--------|
| Tetrahedron | 2 | $4\pi$ | Pass |
| Cube | 2 | $4\pi$ | Pass |
| Octahedron | 2 | $4\pi$ | Pass |
| Torus | 0 | $0$ | Pass |

---

## Mean Curvature Flow

**Continuous Flow:**

$$\frac{\partial p}{\partial t} = H \cdot N = -\Delta p$$

**Discrete Flow (Implicit Euler):**

$$(M - t L) p^{n+1} = M p^n$$

**Properties:**
- Unconditionally stable
- Smooths geometry
- Preserves volume (approximately)
- Converges to minimal surface

**Applications:**
- Mesh denoising
- Fairing
- Shape optimization
- Surface reconstruction

---

## Implementation Tips

### Robustness

1. **Check for degeneracies:**
   ```cpp
   if (face->area() < 1e-10) {
       // Handle degenerate triangle
   }
   ```

2. **Clamp dot products:**
   ```cpp
   double cosAngle = std::clamp(e1.dot(e2), -1.0, 1.0);
   ```

3. **Handle numerical errors:**
   ```cpp
   if (std::isnan(curvature) || std::isinf(curvature)) {
       curvature = 0.0;
   }
   ```

### Visualization

**Curvature colormap:**
```cpp
Vector3 curvatureColor(double K) {
    // Blue (negative) → White (zero) → Red (positive)
    if (K < 0) {
        double t = std::clamp(-K / K_max, 0.0, 1.0);
        return lerp(WHITE, BLUE, t);
    } else {
        double t = std::clamp(K / K_max, 0.0, 1.0);
        return lerp(WHITE, RED, t);
    }
}
```

---

## Exercises

1. **Compute curvature** for platonic solids, verify Gauss-Bonnet
2. **Run mean curvature flow** on noisy mesh, observe smoothing
3. **Compare normals:** vertex normal vs face normal methods
4. **Curvature evolution:** Track total curvature during flow
5. **Boundary effects:** How does curvature behave at boundaries?

---

## Related Chapters

- **Chapter 2:** Combinatorial Surfaces (mesh structure)
- **Chapter 6:** The Laplacian (cotan weights)
- **Chapter 7:** Parameterization (conformal geometry)

## Assignment

- **A2: Curvature** - Implement normal computation, Gaussian curvature

---

**References:**
- Meyer et al., "Discrete Differential-Geometry Operators" (2003)
- Desbrun et al., "Implicit Fairing of Irregular Meshes" (1999)
