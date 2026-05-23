# DDG Course Companion

Companion implementations and notes for Keenan Crane's *Discrete Differential Geometry* course.

> Current state: educational and improving, not production-complete.

[![C++ Build](https://github.com/rhughes42/discrete-differential-geometry-basics/actions/workflows/cpp-build.yml/badge.svg)](https://github.com/rhughes42/discrete-differential-geometry-basics/actions/workflows/cpp-build.yml)
[![C# Build](https://github.com/rhughes42/discrete-differential-geometry-basics/actions/workflows/csharp-build.yml/badge.svg)](https://github.com/rhughes42/discrete-differential-geometry-basics/actions/workflows/csharp-build.yml)
[![WASM Build](https://github.com/rhughes42/discrete-differential-geometry-basics/actions/workflows/wasm-build.yml/badge.svg)](https://github.com/rhughes42/discrete-differential-geometry-basics/actions/workflows/wasm-build.yml)

## Status Snapshot

| Area | Status | Notes |
|---|---|---|
| C++ core mesh + DDG algorithms | **Partial** | Implemented and buildable; some algorithms use simplified operators/solvers |
| C# core mesh + DDG algorithms | **Partial** | All six algorithm modules exist; several contain placeholder/simplified steps |
| Python tooling | **Experimental** | Lightweight visualization script only |
| Web companion | **Experimental** | WASM demo/benchmark pages, no full `web/index.html` app |
| Test coverage | **Partial** | Good basic unit tests; limited degeneracy/non-manifold stress coverage |

See `docs/status/IMPLEMENTATION_AUDIT.md` for the detailed audit.

## Implemented Algorithms by Language

| Algorithm | C++ | C# | Maturity |
|---|:---:|:---:|---|
| Cotan Laplacian | ✅ | ✅ | Partial (C# currently relies on simplified edge cotan path) |
| Mean Curvature Flow | ✅ | ✅ | Partial |
| Discrete Gaussian Curvature | ✅ | ✅ | Partial |
| Conformal Parameterization | ✅ | ✅ | Partial/Experimental |
| Heat Method | ✅ | ✅ | Partial/Experimental |
| Hodge Decomposition | ✅ | ✅ | Experimental |

Legend: **Complete** = solidly validated and robust, **Partial** = implemented with known gaps, **Experimental** = scaffold/initial implementation, **Planned** = not yet implemented.

## Quick Start

### C++

```bash
cd src/cpp
cmake -S . -B build -DCMAKE_BUILD_TYPE=Release
cmake --build build -j4
cd build
ctest --output-on-failure
```

### C#

```bash
cd src/csharp
dotnet restore
dotnet build --configuration Release
cd Tests
dotnet test --configuration Release
```

### WebAssembly

WASM bindings and demos live under `src/wasm/` and `web/wasm-demo.html` / `web/wasm-benchmark.html`.

## Repository Layout

```text
src/cpp/        C++ core mesh, algorithms, tests, examples
src/csharp/     C# core mesh, algorithms, tests, CLI examples
src/wasm/       Emscripten bindings and wasm build config
docs/           Chapters, formulas, assignments, tutorials, status docs
examples/       Example usage notes and Python helper script
web/            Static WASM demo pages
```

## Documentation

- `docs/README.md`
- `docs/algorithms/README.md`
- `docs/formulas/index.md`
- `docs/assignments/README.md`
- `docs/tutorials/README.md`
- `docs/status/IMPLEMENTATION_AUDIT.md`
- `docs/status/NEXT_RELEASE_PLAN.md`

## Roadmap (Realistic)

### 1.1.x Stabilization (current)
- Mesh robustness and topology validation improvements
- Build/test reliability fixes
- Documentation honesty and audit coverage
- Better numerical safeguards in current algorithms

### 1.2.0 Target
- Reusable Poisson solve infrastructure
- ARAP deformation scaffold (explicitly experimental)
- Expanded degeneracy/non-manifold test set
- Better parity notes across C++/C#/WASM

## Contributing

See `CONTRIBUTING.md` for build/test workflow, contribution expectations, and documentation standards.

## License

MIT (`LICENSE`).
