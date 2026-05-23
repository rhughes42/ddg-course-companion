# DDG Course Companion - Project Summary

This repository is an educational DDG implementation project with C++, C#, and WASM components plus supporting docs.

## Technical Reality (v1.1.x pre-release state)

- Core mesh structures exist in C++ and C#.
- Six major DDG algorithm modules exist in both languages.
- Several implementations are still simplified or partial (especially conformal/heat/hodge internals in C# and hodge internals in C++).
- CI workflows exist for C++, C#, and WASM.
- Unit tests cover core mesh/laplacian/curvature properties but do not yet fully cover all robustness scenarios.

## Maturity Labels

| Label | Meaning |
|---|---|
| Complete | Implemented, tested, and robust for expected educational/engineering usage |
| Partial | Implemented with known correctness/robustness/test gaps |
| Experimental | Early scaffold, incomplete numerics/topology handling |
| Planned | Intended but not implemented |

## Algorithm Matrix

| Algorithm | C++ | C# | Current Maturity |
|---|:---:|:---:|---|
| Cotan Laplacian | ✅ | ✅ | Partial |
| Mean Curvature Flow | ✅ | ✅ | Partial |
| Discrete Gaussian Curvature | ✅ | ✅ | Partial |
| Conformal Parameterization | ✅ | ✅ | Partial/Experimental |
| Heat Method | ✅ | ✅ | Partial/Experimental |
| Hodge Decomposition | ✅ | ✅ | Experimental |

## Build and Test Commands

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

### WASM
```bash
cd src/wasm
./build.sh
```

## High-Priority Next Steps

1. Complete robust topology validation and degeneracy handling.
2. Tighten numerical safety around sparse solves and cotangent assembly.
3. Add reusable Poisson infrastructure and stronger regression tests.
4. Keep docs and maturity labels synchronized with implementation reality.

See `docs/status/IMPLEMENTATION_AUDIT.md` and `docs/status/NEXT_RELEASE_PLAN.md`.
