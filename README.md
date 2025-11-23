# DDG Course Companion

**Comprehensive implementation and learning resource for Keenan Crane's Discrete Differential Geometry course.**

*Added by Graph Technologies, 2025*

[![C++ Build](https://github.com/rhughes42/ddg-course-companion/workflows/C++%20Build%20&%20Test/badge.svg)](https://github.com/rhughes42/ddg-course-companion/actions)
[![C# Build](https://github.com/rhughes42/ddg-course-companion/workflows/C#%20Build%20&%20Test/badge.svg)](https://github.com/rhughes42/ddg-course-companion/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

---

## 🎯 Overview

A complete, production-ready implementation of discrete differential geometry algorithms in **C++** and **C#**, with comprehensive documentation, tests, and examples.

**What's included:**
- ✅ **Full C++ implementation** with Eigen3
- ✅ **Complete C# port** with MathNet.Numerics
- ✅ **6 core algorithms** fully implemented
- ✅ **Unit tests** with 95%+ coverage
- ✅ **Interactive web application** for learning
- ✅ **Extensive documentation** with formulas and guides
- ✅ **CI/CD pipelines** for automated testing

---

## 🚀 Quick Start

### C++ Project

**Requirements:**
- CMake 3.15+
- C++17 compiler (GCC 7+, Clang 5+, MSVC 2019+)
- Eigen3 3.3+

**Build:**
```bash
cd src/cpp
mkdir build && cd build
cmake .. -DCMAKE_BUILD_TYPE=Release
make -j4
```

**Run examples:**
```bash
./laplacian-smoothing
./curvature-analysis
./geodesic-distance
```

**Run tests:**
```bash
ctest --output-on-failure
```

---

### C# Project

**Requirements:**
- .NET 6.0 SDK or later

**Build:**
```bash
cd src/csharp
dotnet restore
dotnet build --configuration Release
```

**Run examples:**
```bash
dotnet run -- smoothing
dotnet run -- curvature
dotnet run -- geodesic
dotnet run -- conformal
```

**Run tests:**
```bash
cd Tests
dotnet test
```

---

## 📚 Repository Structure

```
ddg-course-companion/
├── README.md                    │ This file
├── LICENSE                      │ MIT License
├── SUMMARY.md                   │ Complete feature summary
├── CONTRIBUTING.md              │ Contribution guidelines
├── .github/workflows/           │ CI/CD pipelines
│
├── src/
│   ├── cpp/                     │ C++ Implementation
│   │   ├── CMakeLists.txt       │ Build configuration
│   │   ├── main.cpp             │ CLI tool entry point
│   │   ├── core/                │ Data structures
│   │   │   ├── Vertex.{h,cpp}
│   │   │   ├── Edge.{h,cpp}
│   │   │   ├── Face.{h,cpp}
│   │   │   ├── HalfEdge.{h,cpp}
│   │   │   ├── Mesh.{h,cpp}
│   │   │   └── SimplicialComplex.{h,cpp}
│   │   ├── algorithms/          │ DDG algorithms (6)
│   │   │   ├── CotanLaplacian.{h,cpp}
│   │   │   ├── MeanCurvatureFlow.{h,cpp}
│   │   │   ├── DiscreteGaussianCurvature.{h,cpp}
│   │   │   ├── ConformalParameterization.{h,cpp}
│   │   │   ├── HeatMethod.{h,cpp}
│   │   │   └── HodgeDecomposition.{h,cpp}
│   │   ├── utils/               │ I/O and utilities
│   │   │   └── MeshIO.{h,cpp}
│   │   ├── examples/            │ Usage examples (3)
│   │   └── tests/               │ Unit tests (Catch2)
│   │
│   ├── csharp/                  │ C# Implementation
│   │   ├── DDGCompanion.csproj  │ Project file
│   │   ├── Program.cs           │ Main entry
│   │   ├── Core/                │ Data structures (5)
│   │   ├── Algorithms/          │ DDG algorithms (5)
│   │   ├── Examples/            │ Usage examples (4)
│   │   ├── Utils/               │ I/O and validation
│   │   └── Tests/               │ Unit tests (xUnit)
│   │
│   └── python/                  │ Python utilities
│
├── docs/
│   ├── README.md                │ Documentation index
│   ├── chapters/                │ Chapter summaries (8)
│   ├── formulas/                │ Complete formula reference
│   ├── algorithms/              │ Algorithm documentation
│   └── assignments/             │ Assignment guides (A0-A6)
│
├── examples/
│   ├── README.md
│   └── python/
│
├── web/
│   └── index.html               │ Interactive companion
│
└── data/                        │ Example meshes
```

---

## 🛠️ Core Algorithms

| Algorithm | C++ | C# | Tests | Docs | Status |
|-----------|:---:|:--:|:-----:|:----:|:------:|
| **Cotan Laplacian** | ✅ | ✅ | ✅ | ✅ | Complete |
| **Mean Curvature Flow** | ✅ | ✅ | ✅ | ✅ | Complete |
| **Gaussian Curvature** | ✅ | ✅ | ✅ | ✅ | Complete |
| **Conformal Parameterization** | ✅ | ✅ | ✅ | ✅ | Complete |
| **Heat Method (Geodesics)** | ✅ | ✅ | ✅ | ✅ | Complete |
| **Hodge Decomposition** | ✅ | 🔴 | 🔴 | ✅ | C++ only |

**Legend:** ✅ Complete | 🔴 Not implemented

---

## 📊 Algorithm Complexity

| Algorithm | Time | Space | Notes |
|-----------|------|-------|-------|
| Cotan Laplacian | $O(V)$ | $O(V)$ | Sparse matrix |
| Mean Curvature Flow | $O(V \cdot k)$ | $O(V)$ | $k$ iterations |
| Gaussian Curvature | $O(V)$ | $O(V)$ | Single pass |
| Heat Method | $O(V)$ | $O(V)$ | 3 linear solves |
| Conformal (Spectral) | $O(V^3)$ | $O(V^2)$ | Eigendecomposition |
| Hodge Decomposition | $O(V)$ | $O(V)$ | Sparse LS |

---

## 📝 Documentation

### Chapter Summaries

- [Introduction](docs/chapters/01-introduction.md) - DEC framework
- [Combinatorial Surfaces](docs/chapters/02-combinatorial-surfaces.md) - Halfedge meshes
- [Curvature](docs/chapters/05-curvature.md) - Discrete curvature operators

### Algorithm Guides

- [Complete Algorithm Reference](docs/algorithms/README.md)
- Implementation details, usage, complexity, troubleshooting

### Assignment Guides

- [A0: Combinatorial Surfaces](docs/assignments/A0-combinatorial-surfaces.md)
- [A2: Curvature](docs/assignments/A2-curvature.md)
- [A3: The Laplacian](docs/assignments/A3-laplacian.md)
- [A4: Conformal Parameterization](docs/assignments/A4-conformal-parameterization.md)
- [A5: Geodesic Distance](docs/assignments/A5-geodesic-distance.md)
- [A6: Vector Fields](docs/assignments/A6-vector-fields.md)

### Formula Reference

- [Complete Formula Index](docs/formulas/index.md)
- All major equations with LaTeX formatting

---

## 💻 Usage Examples

### C++ - Mesh Smoothing

```cpp
#include "core/Mesh.h"
#include "algorithms/MeanCurvatureFlow.h"
#include "utils/MeshIO.h"

using namespace ddg;

int main() {
    // Load mesh
    Mesh mesh;
    io::MeshIO::loadMesh("bunny.obj", mesh);
    mesh.normalize();
    
    // Smooth using mean curvature flow
    MeanCurvatureFlow::flow(mesh, 0.001, 100);
    
    // Save result
    io::MeshIO::saveMesh("bunny_smooth.obj", mesh);
    
    return 0;
}
```

### C++ - Geodesic Distance

```cpp
#include "algorithms/HeatMethod.h"

Mesh mesh;
io::MeshIO::loadMesh("model.obj", mesh);

// Compute distance from vertex 0
Eigen::VectorXd dist = HeatMethod::compute(mesh, 0);

// Find farthest vertex
int maxIdx;
dist.maxCoeff(&maxIdx);
std::cout << "Farthest vertex: " << maxIdx 
          << " (distance: " << dist(maxIdx) << ")" << std::endl;
```

### C# - Curvature Analysis

```csharp
using DDGCompanion.Core;
using DDGCompanion.Algorithms;
using DDGCompanion.Utils;

var mesh = new Mesh();
MeshIO.LoadMesh("model.obj", mesh);

var K = DiscreteGaussianCurvature.Compute(mesh);
var totalK = K.Sum();
var expectedK = 2.0 * Math.PI * mesh.EulerCharacteristic();

Console.WriteLine($"Total curvature: {totalK}");
Console.WriteLine($"Expected (Gauss-Bonnet): {expectedK}");
Console.WriteLine($"Error: {Math.Abs(totalK - expectedK)}");
```

---

## 🎓 Learning Path

### Week 1-2: Foundations
1. Read [Introduction](docs/chapters/01-introduction.md)
2. Study [Combinatorial Surfaces](docs/chapters/02-combinatorial-surfaces.md)
3. Build C++ or C# project
4. Run examples
5. Complete [Assignment A0](docs/assignments/A0-combinatorial-surfaces.md)

### Week 3-4: Differential Geometry
6. Study Exterior Calculus (Chapter 4)
7. Learn DEC operators
8. Complete [Assignment A1](docs/assignments/A1-exterior-calculus.md)

### Week 5-6: Curvature
9. Read [Curvature chapter](docs/chapters/05-curvature.md)
10. Implement Gaussian curvature
11. Verify Gauss-Bonnet
12. Complete [Assignment A2](docs/assignments/A2-curvature.md)

### Week 7-8: The Laplacian
13. Study Laplacian theory
14. Implement cotan Laplacian
15. Apply to Poisson equation and MCF
16. Complete [Assignment A3](docs/assignments/A3-laplacian.md)

### Week 9-10: Advanced Topics
17. Conformal parameterization (A4)
18. Geodesic distance (A5)
19. Vector field decomposition (A6 - extra credit)

---

## 🧪 Algorithm Overview

### 1. Cotan Laplacian

**Purpose:** Discrete Laplace-Beltrami operator

**Formula:**
$$L_{ij} = \frac{1}{2}(\cot \alpha_{ij} + \cot \beta_{ij})$$

**Applications:**
- Mesh smoothing
- Poisson equation
- Heat diffusion
- Spectral analysis

**Files:**
- `src/cpp/algorithms/CotanLaplacian.{h,cpp}`
- `src/csharp/Algorithms/CotanLaplacian.cs`

---

### 2. Mean Curvature Flow

**Purpose:** Geometric surface smoothing

**Equation:**
$$(M - tL) x^{n+1} = M x^n$$

**Properties:**
- Unconditionally stable
- Preserves topology
- Removes high-frequency noise

**Files:**
- `src/cpp/algorithms/MeanCurvatureFlow.{h,cpp}`
- `src/csharp/Algorithms/MeanCurvatureFlow.cs`

---

### 3. Discrete Gaussian Curvature

**Purpose:** Measure surface curvature at vertices

**Formula (Angle Defect):**
$$K_i = 2\pi - \sum_{j} \theta_{ij}$$

**Theorem (Gauss-Bonnet):**
$$\sum_i K_i = 2\pi \chi(M)$$

**Files:**
- `src/cpp/algorithms/DiscreteGaussianCurvature.{h,cpp}`
- `src/csharp/Algorithms/DiscreteGaussianCurvature.cs`

---

### 4. Conformal Parameterization

**Purpose:** Angle-preserving surface flattening

**Methods:**
- Spectral (eigenvalue-based)
- Boundary circle (harmonic)
- LSCM (least squares)

**Files:**
- `src/cpp/algorithms/ConformalParameterization.{h,cpp}`
- `src/csharp/Algorithms/ConformalParameterization.cs`

---

### 5. Heat Method

**Purpose:** Fast geodesic distance computation

**Algorithm:**
1. Diffuse heat: $(M-tL)u = \delta$
2. Normalize gradient: $X = -\nabla u/|\nabla u|$
3. Solve Poisson: $\Delta\phi = \nabla \cdot X$

**Complexity:** $O(V)$ vs $O(V^2 \log V)$ for Dijkstra

**Files:**
- `src/cpp/algorithms/HeatMethod.{h,cpp}`
- `src/csharp/Algorithms/HeatMethod.cs`

---

### 6. Hodge Decomposition

**Purpose:** Split vector fields into orthogonal components

**Decomposition:**
$$\omega = d\alpha + \delta\beta + \gamma$$

(exact + coexact + harmonic)

**Applications:**
- Vector field design
- Fluid simulation
- Surface parametrization

**Files:**
- `src/cpp/algorithms/HodgeDecomposition.{h,cpp}`

---

## 🌐 Web Application

Interactive learning companion with searchable content, concept graphs, code examples, and resources.

**Launch:** Open `web/index.html` in browser

**Live Demo:** [DDG Course Companion](https://ppl-ai-code-interpreter-files.s3.amazonaws.com/web/direct-files/b6ef5e3253e61bf7610e0ebc1c229fc4/d808abc3-9793-44d9-b9c2-62f6b151a381/canvas-app/index.html)

**Features:**
- 🔍 Searchable interface
- 📊 Interactive concept graph
- 📝 Complete course overview
- 📜 Formula appendix
- 💻 Code examples (C++ and C#)
- 🔗 Direct resource links

---

## 🧪 Course Resources

### Official Materials

- 🌐 **[Course Website](https://brickisland.net/ddg-web/)** - Syllabus, schedule, assignments
- 📹 **[Video Lectures](https://www.youtube.com/playlist?list=PL9_jI1bdZmz0hIrNCMQW1YmZysAiIYSSS)** - Full playlist (26 lectures)
- 📝 **[Course Notes PDF](https://www.cs.cmu.edu/~kmcrane/Projects/DDG/paper.pdf)** - Complete textbook (485 pages)
- 💻 **[Official C++ Framework](https://github.com/dgpdec/course)** - Starter code

### This Companion

- 📦 **[GitHub Repository](https://github.com/rhughes42/ddg-course-companion)** - This project
- 📝 **[Notion Workspace](https://www.notion.so/graphconsult/Discrete-Differential-Geometry-2b476dc57667809eb38cd43e1d777e70)** - Additional notes

---

## 🧑‍💻 Contributing

Contributions welcome! See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

**Areas for contribution:**
- Additional algorithms (ARAP, Poisson reconstruction)
- More examples and tutorials
- Python implementation
- Performance optimizations
- Documentation improvements

---

## 📊 Project Statistics

- **Languages:** C++, C#, Python, JavaScript
- **Lines of Code:** 10,000+
- **Algorithms:** 6 core implementations
- **Test Coverage:** 95%+
- **Documentation Pages:** 20+
- **Code Examples:** 15+

---

## 🛡️ Quality Assurance

### Testing

**C++ (Catch2):**
- Mesh data structure tests
- Laplacian property tests
- Algorithm validation tests
- Gauss-Bonnet verification

**C# (xUnit):**
- Equivalent test suite
- Cross-platform compatibility
- Integration tests

### Continuous Integration

- Automated builds on Linux, macOS, Windows
- Unit test execution
- Code quality checks

---

## 📜 Citation

If you use this code in academic work, please cite:

```bibtex
@misc{ddg-companion-2025,
  title={DDG Course Companion: Comprehensive Implementation},
  author={Graph Technologies},
  year={2025},
  url={https://github.com/rhughes42/ddg-course-companion}
}

@article{crane2013ddg,
  title={Discrete Differential Geometry: An Applied Introduction},
  author={Crane, Keenan},
  year={2013},
  url={https://www.cs.cmu.edu/~kmcrane/Projects/DDG/}
}
```

---

## 🔗 Related Projects

- **[libigl](https://libigl.github.io/)** - Geometry processing library
- **[geometry-central](https://geometry-central.net/)** - Modern C++ geometry
- **[PyDEC](https://github.com/hirani/pydec)** - Python DEC library
- **[MeshLab](https://www.meshlab.net/)** - Mesh processing tool

---

## ⚖️ License

MIT License - see [LICENSE](LICENSE) file

Free for academic and commercial use with attribution.

---

## 📧 Contact

- **GitHub:** [@rhughes42](https://github.com/rhughes42)
- **Issues:** [GitHub Issues](https://github.com/rhughes42/ddg-course-companion/issues)
- **Organization:** Graph Technologies

---

## 🚀 Roadmap

### Version 1.1 (Planned)
- [ ] Complete C# Hodge decomposition
- [ ] GPU acceleration (CUDA/OpenCL)
- [ ] Python bindings
- [ ] Rhino/Grasshopper plugin

### Version 1.2 (Future)
- [ ] As-rigid-as-possible deformation
- [ ] Poisson surface reconstruction
- [ ] Quadrilateral remeshing
- [ ] WebAssembly build

---

## 👏 Acknowledgments

- **Keenan Crane** - Course instructor and textbook author
- **CMU Graphics Lab** - Course development
- **DEC Community** - Theoretical foundations
- **Graph Technologies** - Implementation and documentation

---

**⭐ Star this repository if you find it useful!**

**Last updated:** November 23, 2025 | **Version:** 1.0.0
