# Examples

Usage examples for DDG algorithms.

## C++ Examples

### Load and Process Mesh

```cpp
#include "core/Mesh.h"
#include "algorithms/CotanLaplacian.h"

int main() {
    // Load mesh (assuming OBJ loader)
    Eigen::MatrixXd V;
    Eigen::MatrixXi F;
    // ... load from file ...
    
    Mesh mesh;
    mesh.build(V, F);
    mesh.normalize();  // Center and scale to unit sphere
    
    // Smooth mesh
    MeanCurvatureFlow::flow(mesh, 0.001, 50);
    
    // Save result
    Eigen::MatrixXd V_smooth = mesh.vertexPositions();
    // ... save to file ...
}
```

### Compute Curvature

```cpp
#include "algorithms/DiscreteGaussianCurvature.h"

Eigen::VectorXd K = DiscreteGaussianCurvature::compute(mesh);

// Visualize by mapping to colors
for (int i = 0; i < K.size(); i++) {
    double k = K(i);
    // Map curvature to color
    // Positive = red, Negative = blue, Zero = white
}
```

### Solve Poisson Equation

```cpp
// Set up right-hand side
Eigen::VectorXd f = Eigen::VectorXd::Random(mesh.numVertices());

// Solve Δu = f
Eigen::VectorXd u = CotanLaplacian::solvePoisson(mesh, f);
```

## C# Examples

### Build Mesh

```csharp
using DDGCompanion.Core;
using System.Numerics;

var mesh = new Mesh();

// Add vertices
mesh.Vertices.Add(new Vertex(new Vector3(0, 0, 0)));
mesh.Vertices.Add(new Vertex(new Vector3(1, 0, 0)));
mesh.Vertices.Add(new Vertex(new Vector3(0, 1, 0)));

// Add faces
// ... (similar to C++) ...
```

### Compute Laplacian

```csharp
using DDGCompanion.Algorithms;
using MathNet.Numerics.LinearAlgebra;

var L = CotanLaplacian.Build(mesh);
var M = CotanLaplacian.BuildMassMatrix(mesh);

// Solve system
var solver = L.SolveCholesky();
var solution = solver.Solve(rhs);
```

## Python Utilities

### Visualize Results

```python
import numpy as np
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D

# Load vertices and faces
V = np.loadtxt('vertices.txt')
F = np.loadtxt('faces.txt', dtype=int)

# Plot mesh
fig = plt.figure()
ax = fig.add_subplot(111, projection='3d')
ax.plot_trisurf(V[:,0], V[:,1], V[:,2], triangles=F)
plt.show()
```

### Compute Statistics

```python
def mesh_statistics(V, F):
    n_vertices = V.shape[0]
    n_faces = F.shape[0]
    n_edges = n_faces + n_vertices - 2  # Euler formula
    
    print(f"Vertices: {n_vertices}")
    print(f"Edges: {n_edges}")
    print(f"Faces: {n_faces}")
    print(f"Euler characteristic: {n_vertices - n_edges + n_faces}")
```

## Assignment Examples

See `docs/assignments/` for detailed implementation guides for each assignment.

## Test Meshes

Common test meshes:
- Tetrahedron: 4 vertices, 6 edges, 4 faces
- Cube: 8 vertices, 12 edges, 6 faces
- Icosahedron: 12 vertices, 30 edges, 20 faces
- Sphere (subdivided icosahedron)
- Bunny, Armadillo, Dragon (Stanford models)
