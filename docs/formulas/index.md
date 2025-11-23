# Formula Reference

Complete reference of all major formulas in DDG.

## Chapter 1: Introduction

*No major formulas*

## Chapter 2: Combinatorial Surfaces

### Euler Characteristic

$$\chi = V - E + F$$

For closed surfaces:
$$\chi = 2 - 2g$$

where $g$ is the genus.

### Mean Valence

$$\bar{d} = \frac{2E}{V}$$

For triangulated surface:
$$\bar{d} = 6 - \frac{12(1-g)}{V}$$

## Chapter 3: Differential Geometry

### First Fundamental Form

$$I = E\,du^2 + 2F\,du\,dv + G\,dv^2$$

where $E = \langle f_u, f_u \rangle$, $F = \langle f_u, f_v \rangle$, $G = \langle f_v, f_v \rangle$

### Gaussian Curvature

$$K = \kappa_1 \kappa_2$$

where $\kappa_1, \kappa_2$ are principal curvatures.

### Mean Curvature

$$H = \frac{\kappa_1 + \kappa_2}{2}$$

## Chapter 4: Exterior Calculus

### Exterior Derivative

$$d(\alpha \wedge \beta) = d\alpha \wedge \beta + (-1)^k \alpha \wedge d\beta$$

where $\alpha$ is a $k$-form.

### Stokes' Theorem

$$\int_{\partial\sigma} \omega = \int_\sigma d\omega$$

Fundamental theorem relating integration and differentiation.

### Hodge Star

$$\star\star\omega = (-1)^{k(n-k)} \omega$$

for $k$-form $\omega$ in $n$-dimensional space.

### Codifferential

$$\delta = (-1)^{nk+n+1} \star d \star$$

## Chapter 5: Curvature

### Angle Defect (Discrete Gaussian Curvature)

$$K_i = 2\pi - \sum_j \theta_{ij}$$

Sum of angles at vertex $i$.

### Discrete Gauss-Bonnet

$$\sum_{i=1}^V K_i = 2\pi\chi(M)$$

Relates curvature to topology.

### Area Gradient (Cotan Formula)

$$\nabla A_i = \frac{1}{4} \sum_j (\cot \alpha_{ij} + \cot \beta_{ij})(p_j - p_i)$$

## Chapter 6: The Laplacian

### Cotan Laplacian

$$L_{ij} = \begin{cases}
\frac{1}{2}(\cot \alpha_{ij} + \cot \beta_{ij}) & \text{if } i \sim j \\
-\sum_{k} L_{ik} & \text{if } i = j \\
0 & \text{otherwise}
\end{cases}$$

### Poisson Equation

$$\Delta u = f \quad \Rightarrow \quad Lu = Mf$$

where $M$ is the mass matrix (diagonal, vertex areas).

### Implicit Mean Curvature Flow

$$(M - tL)x^{n+1} = Mx^n$$

Stable integration scheme.

## Chapter 7: Surface Parameterization

### Conformal Energy (Dirichlet Energy)

$$E_D(f) = \int_M |df|^2 = \int_M |\nabla f|^2$$

### Cauchy-Riemann Equations

$$\frac{\partial u}{\partial x} = \frac{\partial v}{\partial y}, \quad \frac{\partial u}{\partial y} = -\frac{\partial v}{\partial x}$$

Condition for conformal map $f = u + iv$.

### Spectral Conformal Parameterization

$$L\phi = \lambda M\phi$$

Eigenvectors give conformal coordinates.

## Chapter 8: Vector Fields

### Hodge Decomposition

$$\omega = d\alpha + \delta\beta + \gamma$$

Decomposition into exact, coexact, and harmonic components.

### Helmholtz Equation

$$\Delta\omega = 0 \quad \Leftrightarrow \quad d\omega = 0 \text{ and } \delta\omega = 0$$

Harmonic forms.

### Betti Numbers

$$\beta_k = \dim H^k(M)$$

Dimension of $k$-th cohomology group.

For closed orientable surface:
- $\beta_0 = 1$ (connected components)
- $\beta_1 = 2g$ (loops)
- $\beta_2 = 1$ (volume)

## Notation

- $M$: Manifold (surface)
- $\sigma$: Simplex
- $\omega, \alpha, \beta$: Differential forms
- $d$: Exterior derivative
- $\star$: Hodge star
- $\delta$: Codifferential
- $\Delta$: Laplace-Beltrami operator
- $K$: Gaussian curvature
- $H$: Mean curvature
- $\chi$: Euler characteristic
- $g$: Genus
