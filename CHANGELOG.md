# Changelog

## [Unreleased]

### Changed
- Replaced overstated top-level docs with explicit maturity labels.
- Updated C++/C#/WASM workflow action versions and corrected C# test execution path.
- Improved C++ CMake warning defaults and fixed broken C++ test target list.

### Added
- `docs/status/IMPLEMENTATION_AUDIT.md`
- `docs/status/NEXT_RELEASE_PLAN.md`
- `docs/tutorials/README.md`
- C# invalid topology input test in `MeshTests`

### Fixed
- C# mesh build now validates triangle input and invalid indices.
- C# vertex traversal methods now guard against null twin/next paths.
- C# cotan Laplacian no longer uses fixed placeholder weight.
- C# edge cotangent calculation now guards against near-zero denominators.
- Oriented tetrahedron fixtures updated in C++ and C# tests for consistent closed-mesh topology.
