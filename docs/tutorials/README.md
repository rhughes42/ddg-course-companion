# Tutorials

This tutorial index is intentionally concise and aligned with the current implementation maturity.

## 1) Loading a Mesh
- **Goal:** Build halfedge mesh from vertex/face arrays.
- **Theory:** Triangle mesh connectivity and oriented halfedges.
- **Code path:** `src/cpp/core/Mesh.cpp`, `src/csharp/Core/Mesh.cs`.
- **Failure modes:** Invalid indices, degenerate triangle edges, inconsistent winding.

## 2) Validating Topology
- **Goal:** Check whether input is suitable for DDG operators.
- **Theory:** Manifoldness, orientation consistency, boundary awareness.
- **Current state:** Partial validation (invalid index and basic traversal safety).
- **Next topic:** Add explicit non-manifold and duplicate-edge diagnostics.

## 3) Computing Gaussian Curvature
- **Goal:** Use angle defect at vertices.
- **Theory:** \(K_i = 2\pi - \sum\theta_i\), Gauss-Bonnet.
- **Code path:** `DiscreteGaussianCurvature` in C++/C#.
- **Numerical notes:** Clamp dot products; guard zero-length edges.

## 4) Constructing the Cotan Laplacian
- **Goal:** Assemble sparse Laplace-Beltrami matrix.
- **Theory:** Cotangent weights over one-ring neighborhood.
- **Code path:** `CotanLaplacian` in C++/C#.
- **Failure modes:** Broken topology traversal, degenerate triangles causing unstable cotangents.

## 5) Solving Poisson Equations
- **Goal:** Solve \(\Delta u = f\) on meshes.
- **Theory:** Sparse linear systems with Laplacian and mass matrix.
- **Code path:** `solvePoisson` helper in C++ cotan module.
- **Current maturity:** Partial; reusable constrained API is planned.

## 6) Mean Curvature Flow
- **Goal:** Implicit smoothing using \((M - tL)x^{n+1}=Mx^n\).
- **Code path:** `MeanCurvatureFlow` in C++/C#.
- **Numerical notes:** choose stable timestep and monitor shrinkage.

## 7) Heat Method Geodesics
- **Goal:** Approximate geodesic distance from sources.
- **Code path:** `HeatMethod` in C++/C#.
- **Current maturity:** Partial; C# divergence stage remains simplified.

## 8) Understanding DEC Operators
- **Goal:** Build `d0`, `d1`, and Hodge stars for decomposition workflows.
- **Code path:** `HodgeDecomposition` in C++/C#.
- **Current maturity:** Experimental; harmonic basis generation is incomplete.
