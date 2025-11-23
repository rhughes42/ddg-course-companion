#!/bin/bash
# build.sh
# WebAssembly build script
# Added by Graph Technologies, 2025

set -e

echo "================================"
echo "DDG WebAssembly Build"
echo "Added by Graph Technologies"
echo "================================"
echo ""

# Check for Emscripten
if ! command -v emcc &> /dev/null; then
    echo "Error: Emscripten not found!"
    echo "Please install Emscripten SDK:"
    echo "  git clone https://github.com/emscripten-core/emsdk.git"
    echo "  cd emsdk"
    echo "  ./emsdk install latest"
    echo "  ./emsdk activate latest"
    echo "  source ./emsdk_env.sh"
    exit 1
fi

echo "Emscripten version:"
emcc --version
echo ""

# Create build directory
mkdir -p build
cd build

# Configure with Emscripten
echo "Configuring..."
emcmake cmake .. \
    -DCMAKE_BUILD_TYPE=Release \
    -DEIGEN3_INCLUDE_DIR=/usr/include/eigen3

echo ""
echo "Building..."
emmake make -j4

echo ""
echo "Installing to web/wasm/..."
make install

echo ""
echo "✅ Build complete!"
echo "Output files:"
echo "  - ../../web/wasm/ddg.js"
echo "  - ../../web/wasm/ddg.wasm"
echo ""
echo "File sizes:"
ls -lh ../../web/wasm/ddg.*

echo ""
echo "Usage in HTML:"
echo "  <script src='wasm/ddg.js'></script>"
echo "  <script>"
echo "    DDGModule().then(module => {"
echo "      const mesh = new module.Mesh();"
echo "      // Use mesh..."
echo "    });"
echo "  </script>"
