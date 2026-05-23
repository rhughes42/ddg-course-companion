# Next Release Plan

## Recommended Version Bump

**Recommend: `1.1.1`**

Reasoning:
- This pass focuses on stabilization, test/build reliability fixes, and documentation correctness.
- No intentional breaking API redesign was introduced.
- Major new capabilities like ARAP/Poisson infrastructure are not yet complete enough for `1.2.0`.

## Included in this stabilization pass

- C++ build/test reliability fixes (test target correctness, warning policy improvements).
- C# mesh robustness improvements and defensive traversal safety.
- Test updates for oriented tetrahedra and invalid index rejection.
- CI workflow alignment (action versions, .NET version, test path correctness).
- Documentation audit and maturity relabeling.

## Outstanding Risks

- Heat method and hodge decomposition remain partial/experimental.
- Non-manifold and severe degeneracy handling is still incomplete.
- Cross-language parity and numerical baseline tests are still limited.

## Suggested GitHub Issues

1. Add explicit mesh topology validator (orientation, manifoldness, duplicates).
2. Add Poisson solver API with Dirichlet constraints and tests.
3. Complete C# heat-method divergence stage.
4. Improve Hodge star and harmonic basis implementations.
5. Add stress tests for open meshes, disconnected meshes, and near-degenerate triangles.

## Proposed Milestones

### Milestone: v1.1.1
- Reliability/documentation/test integrity fixes only.

### Milestone: v1.2.0
- Poisson solver infrastructure
- ARAP scaffold
- Expanded algorithm validation suite
