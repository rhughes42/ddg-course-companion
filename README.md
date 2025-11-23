# Discrete Differential Geometry - Course Companion

[![Course](https://img.shields.io/badge/Course-CMU%2015--458-blue)](https://brickisland.net/ddg-web/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

A comprehensive learning companion for Keenan Crane's **Discrete Differential Geometry** course with searchable resources, implementations in C++/C#, and interactive materials.

## 🎯 Purpose

This repository provides:
- **Structured implementations** of core DDG algorithms
- **Well-documented code** in C++ and C# with explanations
- **Summaries and notes** for each course module
- **Formula appendix** with LaTeX equations
- **Interactive web application** for exploring course content
- **Assignment solutions** and implementation guides

## 📚 Repository Structure

```
ddg-course-companion/
├── src/
│   ├── cpp/                    # C++ implementations
│   │   ├── core/              # Core data structures
│   │   ├── algorithms/        # DDG algorithms
│   │   └── assignments/       # Assignment implementations
│   ├── csharp/                # C# implementations
│   │   ├── Core/              # Core data structures
│   │   ├── Algorithms/        # DDG algorithms
│   │   └── Assignments/       # Assignment implementations
│   └── python/                # Python utilities & visualization
├── docs/
│   ├── chapters/              # Chapter summaries
│   ├── formulas/              # Formula reference
│   ├── assignments/           # Assignment guides
│   └── notes/                 # Additional notes
├── web/
│   └── index.html             # Interactive companion app
├── examples/                   # Usage examples
├── tests/                      # Unit tests
└── resources/                  # Additional materials
```

## 🚀 Quick Start

### C++ Implementation

```bash
cd src/cpp
mkdir build && cd build
cmake ..
make
./ddg-demo
```

### C# Implementation

```bash
cd src/csharp
dotnet build
dotnet run --project DDGCompanion
```

### Web Application

Open `web/index.html` in your browser or visit the [live demo](https://ppl-ai-code-interpreter-files.s3.amazonaws.com/web/direct-files/b6ef5e3253e61bf7610e0ebc1c229fc4/d808abc3-9793-44d9-b9c2-62f6b151a381/canvas-app/index.html).

## 📖 Course Coverage

### Chapters

1. **Introduction** - DEC framework, applications
2. **Combinatorial Surfaces** - Simplicial complexes, halfedge meshes
3. **Differential Geometry** - Curves, surfaces, curvature
4. **Exterior Calculus** - Forms, wedge product, Hodge star
5. **Curvature** - Discrete curvature, Gauss-Bonnet
6. **The Laplacian** - Cotan Laplacian, Poisson equation, MCF
7. **Surface Parameterization** - Conformal maps
8. **Vector Fields** - Hodge decomposition, parallel transport

### Assignments Covered

- ✅ **A0**: Combinatorial Surfaces
- ✅ **A1**: Exterior Calculus
- ✅ **A2**: Curvature
- ✅ **A3**: The Laplacian
- ✅ **A4**: Conformal Parameterization
- ✅ **A5**: Geodesic Distance
- ✅ **A6**: Vector Field Decomposition

## 🔧 Key Implementations

### Core Data Structures

- **Halfedge Mesh** - Efficient manifold surface representation
- **Simplicial Complex** - Abstract and geometric complexes
- **Sparse Matrices** - Using Eigen (C++) and MathNet (C#)

### Algorithms

- **Cotan Laplacian** - Discrete Laplace-Beltrami operator
- **Mean Curvature Flow** - Implicit surface smoothing
- **Conformal Parameterization** - Spectral and boundary-constrained
- **Hodge Decomposition** - Vector field decomposition
- **Geodesic Distance** - Heat method implementation
- **Discrete Curvature** - Gaussian and mean curvature

## 📊 Dependencies

### C++
- **Eigen3** - Linear algebra
- **libigl** (optional) - Geometry processing utilities
- **CMake** - Build system

### C#
- **.NET 6+** - Runtime
- **MathNet.Numerics** - Numerical computing
- **System.Numerics** - Vector/matrix operations

### Python
- **NumPy** - Numerical operations
- **SciPy** - Scientific computing
- **Matplotlib** - Visualization

## 📚 Resources

- [Course Website](https://brickisland.net/ddg-web/)
- [Course Notes (PDF)](https://www.cs.cmu.edu/~kmcrane/Projects/DDG/paper.pdf)
- [Video Lectures](https://www.youtube.com/playlist?list=PL9_jI1bdZmz0hIrNCMQW1YmZysAiIYSSS)
- [Official Course Framework](https://github.com/dgpdec/course)
- [Notion Workspace](https://www.notion.so/graphconsult/Discrete-Differential-Geometry-2b476dc57667809eb38cd43e1d777e70)

## 🤝 Contributing

Contributions welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## 📄 License

MIT License - see [LICENSE](LICENSE) for details.

## 🙏 Acknowledgments

- **Keenan Crane** - Course instructor and textbook author
- **CMU Computer Graphics Group** - Course materials
- **DEC Community** - Open source implementations

---

**Note**: This is an educational companion repository. All course materials and concepts are attributed to Keenan Crane and CMU.
