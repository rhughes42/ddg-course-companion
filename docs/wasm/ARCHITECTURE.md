# WebAssembly Architecture

**Added by Graph Technologies, 2025**

Detailed architecture and design decisions for the DDG WebAssembly implementation.

---

## System Overview

```
┌───────────────────────────────────────┐
│         JavaScript Application              │
│    (Browser / Node.js / Web Worker)       │
└──────────────┬─────────────────────────┘
                 │
                 │ Emscripten Bindings
                 │ (--bind API)
                 │
┌──────────────┴─────────────────────────┐
│         WASMMesh Wrapper Class             │
│  (bindings.cpp - JS/C++ interface)      │
└──────────────┬─────────────────────────┘
                 │
                 │ Internal C++ API
                 │
┌──────────────┴─────────────────────────┐
│           Core DDG Library                 │
│    (Mesh, Vertex, Edge, Face, etc.)      │
└──────────────┬─────────────────────────┘
                 │
                 │
┌──────────────┴─────────────────────────┐
│         Algorithm Implementations          │
│  (Laplacian, MCF, Curvature, etc.)      │
└──────────────┬─────────────────────────┘
                 │
                 │
┌──────────────┴─────────────────────────┐
│         Eigen3 Linear Algebra              │
│    (Matrix operations, solvers)          │
└────────────────────────────────────────┘
```

---

## Design Decisions

### 1. Single Wrapper Class Pattern

**Decision:** Use `WASMMesh` wrapper instead of exposing raw C++ classes.

**Rationale:**
- Cleaner JavaScript API
- Encapsulates memory management
- Easier to extend without breaking API
- Reduces binding complexity

**Implementation:**
```cpp
class WASMMesh {
private:
    Mesh mesh;  // Internal C++ mesh
public:
    void buildFromArrays(const val& positions, const val& faces);
    val getPositions() const;
    // ... methods that convert between JS and C++ ...
};
```

---

### 2. Typed Array Interface

**Decision:** Use Float32Array/Int32Array for data transfer.

**Rationale:**
- Zero-copy when possible
- Natural JavaScript API
- Efficient bulk data transfer
- Compatible with WebGL/Three.js

**Example:**
```javascript
const positions = new Float32Array([
    0, 0, 0,  // v0
    1, 0, 0,  // v1
    0, 1, 0   // v2
]);

mesh.buildFromArrays(positions, faces);
```

---

### 3. Modular Build System

**Decision:** Separate WASM build from native C++ build.

**Rationale:**
- Different compiler toolchains
- Different optimization strategies
- Independent CI/CD pipelines
- Easier to maintain

**Structure:**
```
src/
├── cpp/           # Native C++ (CMake)
└── wasm/          # WASM build (Emscripten CMake)
    ├── CMakeLists.txt
    ├── bindings.cpp
    └── build.sh
```

---

### 4. Memory Management Strategy

**Decision:** Automatic via Embind, manual for large arrays.

**Automatic (small data):**
```javascript
const K = mesh.computeGaussianCurvature();
// Embind handles conversion automatically
```

**Manual (large data):**
```javascript
const numFloats = 1000000;
const ptr = module._malloc(numFloats * 4);
const heap = new Float32Array(module.HEAPF32.buffer, ptr, numFloats);
// Use heap...
module._free(ptr);
```

**Rule:** Use automatic for <10K elements, manual for larger.

---

### 5. Error Handling

**Decision:** Return valid data or throw exceptions (never return null/undefined).

**C++ side:**
```cpp
val WASMMesh::computeGaussianCurvature() const {
    if (mesh.vertices.empty()) {
        throw std::runtime_error("Mesh not built");
    }
    // ... compute ...
}
```

**JavaScript side:**
```javascript
try {
    const K = mesh.computeGaussianCurvature();
} catch (e) {
    console.error('Curvature computation failed:', e.message);
}
```

---

## Performance Optimizations

### 1. Compilation Flags

```cmake
-O3                      # Aggressive optimization
-s ALLOW_MEMORY_GROWTH=1 # Dynamic memory
-s MODULARIZE=1          # Clean module pattern
--bind                   # Embind API
```

**Release vs Debug:**

| Flag | Release | Debug |
|------|---------|-------|
| Optimization | -O3 | -O0 |
| Assertions | OFF | ON |
| Source maps | NO | YES |
| Safe heap | NO | YES |
| Size | ~400KB | ~2MB |

---

### 2. Data Transfer Optimization

**Avoid:**
```javascript
// ❌ Creates temporary array, copies data
for (let i = 0; i < mesh.numVertices(); i++) {
    const pos = mesh.getVertexPosition(i);  // Bad: per-vertex call
}
```

**Prefer:**
```javascript
// ✅ Single bulk transfer
const positions = mesh.getPositions();  // Get all at once
for (let i = 0; i < positions.length; i += 3) {
    // Process positions[i], positions[i+1], positions[i+2]
}
```

---

### 3. Algorithm Selection

**For interactive use:**
- ✅ Heat method: $O(V)$ sparse solve
- ❌ Dijkstra: $O(V^2 \log V)$ too slow

**For batch processing:**
- ✅ Spectral conformal: accurate, stable
- ❌ LSCM: more complex, similar results

---

## Memory Layout

### Heap Structure

```
WASM Linear Memory (grows dynamically)
┌────────────────────────────────────────┐
│  Static Data (code, globals)            │
├────────────────────────────────────────┤
│  Stack                                  │
├────────────────────────────────────────┤
│  Heap (malloc/free)                     │
│    - Mesh data structures               │
│    - Eigen matrices                     │
│    - Temporary buffers                  │
│                                          │
│  (grows upward ↑)                      │
└────────────────────────────────────────┘
```

**Initial size:** 16MB
**Growth:** Automatic when needed
**Max size:** 2GB (WASM limit)

---

## Data Conversion Patterns

### JavaScript → C++

```cpp
void WASMMesh::buildFromArrays(const val& positions, const val& faces) {
    int numVertices = positions["length"].as<int>() / 3;
    
    Eigen::MatrixXd V(numVertices, 3);
    for (int i = 0; i < numVertices; i++) {
        V(i, 0) = positions[i * 3 + 0].as<double>();
        V(i, 1) = positions[i * 3 + 1].as<double>();
        V(i, 2) = positions[i * 3 + 2].as<double>();
    }
    
    mesh.build(V, F);
}
```

### C++ → JavaScript

```cpp
val WASMMesh::getPositions() const {
    Eigen::MatrixXd V = mesh.vertexPositions();
    val result = val::array();
    
    for (int i = 0; i < V.rows(); i++) {
        result.call<void>("push", V(i, 0));
        result.call<void>("push", V(i, 1));
        result.call<void>("push", V(i, 2));
    }
    
    return result;
}
```

**Cost:** $O(n)$ copy (unavoidable for Embind)

---

## Threading Considerations

### Web Workers

**Use case:** Heavy computation without blocking UI.

**Setup:**

```javascript
// main.js
const worker = new Worker('ddg-worker.js');

worker.postMessage({
    cmd: 'smooth',
    positions: positions.buffer,
    faces: faces.buffer,
    timestep: 0.001,
    steps: 100
}, [positions.buffer, faces.buffer]);  // Transfer ownership

worker.onmessage = (e) => {
    const smoothed = e.data.positions;
    // Update UI
};
```

```javascript
// ddg-worker.js
importScripts('wasm/ddg.js');

DDGModule().then(module => {
    self.onmessage = (e) => {
        const mesh = new module.Mesh();
        mesh.buildFromArrays(
            new Float32Array(e.data.positions),
            new Int32Array(e.data.faces)
        );
        
        mesh.meanCurvatureFlow(e.data.timestep, e.data.steps);
        
        const result = mesh.getPositions();
        self.postMessage({ positions: result }, [result.buffer]);
    };
});
```

---

### Shared Memory (Advanced)

**Not yet supported** in stable Emscripten + browsers.

Future: Use `SharedArrayBuffer` for zero-copy between threads.

---

## Security Considerations

### 1. Sandboxing

WASM runs in browser sandbox:
- ✅ No file system access
- ✅ No network access
- ✅ No system calls
- ✅ Memory isolated from JavaScript

### 2. CORS Requirements

**WASM files must be served with correct CORS headers:**

```
Cross-Origin-Embedder-Policy: require-corp
Cross-Origin-Opener-Policy: same-origin
```

**nginx config:**
```nginx
location /wasm/ {
    add_header Cross-Origin-Embedder-Policy require-corp;
    add_header Cross-Origin-Opener-Policy same-origin;
    add_header Cache-Control "public, max-age=31536000";
}
```

---

## Browser Compatibility

### Required Features

| Feature | Chrome | Firefox | Safari | Edge |
|---------|--------|---------|--------|------|
| WebAssembly | 57+ | 52+ | 11+ | 16+ |
| BigInt | 67+ | 68+ | 14+ | 79+ |
| Shared Memory | 92+ | 89+ | 15.2+ | 92+ |

### Polyfills

Not needed - WASM has excellent support (98%+ global coverage).

---

## Deployment Strategies

### Strategy 1: Static Hosting

**Pros:**
- Simple
- Fast (CDN)
- Cheap

**Cons:**
- Fixed bundle size
- No server-side processing

**Best for:** Demos, documentation, small apps

---

### Strategy 2: Dynamic Loading

**Idea:** Load algorithms on-demand.

**Implementation:**
```javascript
// Load only what's needed
const laplacianModule = await import('./wasm/laplacian.js');
const heatModule = await import('./wasm/heat-method.js');
```

**Requires:** Separate builds per algorithm.

---

### Strategy 3: Streaming Compilation

**Idea:** Compile WASM while downloading.

```javascript
const { instance } = await WebAssembly.instantiateStreaming(
    fetch('wasm/ddg.wasm'),
    importObject
);
```

**Benefit:** Faster startup on slow connections.

---

## Debugging Tools

### 1. Browser DevTools

**Sources tab:**
- Step through C++ source (with source maps)
- Set breakpoints
- Inspect variables

**Memory profiler:**
- Track WASM heap usage
- Find memory leaks

---

### 2. Emscripten Debugging

**Enable debug info:**
```bash
emcmake cmake .. -DCMAKE_BUILD_TYPE=Debug
```

**Debug output:**
```bash
-s ASSERTIONS=1          # Runtime checks
-s SAFE_HEAP=1           # Catch memory errors
-s STACK_OVERFLOW_CHECK=2 # Stack checks
-g                       # Debug symbols
```

---

### 3. Performance Profiling

**Chrome DevTools Performance tab:**

```javascript
performance.mark('start');
mesh.meanCurvatureFlow(0.001, 100);
performance.mark('end');
performance.measure('MCF', 'start', 'end');
```

**Emscripten profiler:**
```bash
-s EMSCRIPTEN_TRACING=1
```

Generates trace.json for chrome://tracing

---

## Comparison: WASM vs Native

### Performance

**Typical overhead:** 1.5-2× slower than native

**Why:**
- Interpretation overhead
- No SIMD (yet)
- Memory indirection
- Garbage collection pauses

**Mitigations:**
- Use SIMD when available (experimental)
- Batch operations
- Minimize JS ↔ WASM crossings

---

### Features

| Feature | Native C++ | WASM |
|---------|-----------|------|
| File I/O | ✅ Direct | ❌ Requires JS |
| Threading | ✅ Full | 🔶 Limited |
| SIMD | ✅ SSE/AVX | 🔶 Experimental |
| Debugger | ✅ GDB/LLDB | 🔶 DevTools |
| Optimization | ✅ -O3 + LTO | ✅ -O3 |

---

## Future Enhancements

### 1. SIMD Support

```cpp
#ifdef __wasm_simd128__
// Use WASM SIMD intrinsics
#endif
```

**Potential speedup:** 2-4× for linear algebra

---

### 2. Threading (pthread)

```cmake
-s USE_PTHREADS=1
-s PTHREAD_POOL_SIZE=4
```

**Use case:** Parallel matrix assembly, multi-mesh processing

---

### 3. GPU Integration

**WebGPU compute shaders** for massive parallelism:

```javascript
const computePipeline = device.createComputePipeline({
    compute: {
        module: shaderModule,
        entryPoint: 'laplacian'
    }
});

// Run on GPU
computePass.dispatchWorkgroups(Math.ceil(numVertices / 256));
```

**Benefit:** 10-100× speedup for large meshes

---

## Testing Strategy

### Unit Tests (Node.js)

```bash
node tests/run-node-tests.js
```

Runs in CI/CD, verifies correctness.

### Integration Tests (Browser)

Open `wasm-demo.html`, manual verification.

### Performance Tests (Benchmark)

Open `wasm-benchmark.html`, compare times.

---

## References

- **Emscripten:** https://emscripten.org/
- **Embind:** https://emscripten.org/docs/porting/connecting_cpp_and_javascript/embind.html
- **WASM Spec:** https://webassembly.github.io/spec/
- **Performance:** https://web.dev/webassembly/

---

**Built by Graph Technologies, 2025**
