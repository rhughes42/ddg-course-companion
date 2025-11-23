# Contributing to DDG Course Companion

Thank you for your interest in contributing! This document provides guidelines for contributing to the project.

## Ways to Contribute

### 1. Code Contributions

- Implement missing algorithms
- Add new examples
- Improve existing implementations
- Fix bugs
- Optimize performance

### 2. Documentation

- Write chapter summaries
- Add formula explanations
- Create assignment guides
- Improve code comments
- Write tutorials

### 3. Testing

- Add unit tests
- Test on different platforms
- Report bugs
- Verify algorithms

### 4. Resources

- Add useful links
- Share learning materials
- Contribute example meshes
- Create visualizations

## Getting Started

### Fork and Clone

```bash
git clone https://github.com/rhughes42/ddg-course-companion.git
cd ddg-course-companion
git checkout -b feature/your-feature-name
```

### Set Up Development Environment

**C++:**
```bash
cd src/cpp
mkdir build && cd build
cmake ..
make
```

**C#:**
```bash
cd src/csharp
dotnet restore
dotnet build
```

## Code Style

### C++ Style

- Follow Google C++ Style Guide
- Use meaningful variable names
- Comment complex algorithms
- Keep functions focused and short
- Use Eigen for linear algebra

**Example:**

```cpp
// Good
double Edge::cotan() const {
    // Compute cotangent weight for Laplacian
    double cotSum = 0.0;
    
    if (halfedge->face) {
        // Cotan from first adjacent triangle
        Eigen::Vector3d e1 = halfedge->vector();
        Eigen::Vector3d e2 = halfedge->next->vector();
        cotSum += computeCotangent(e1, e2);
    }
    
    return cotSum;
}
```

### C# Style

- Follow Microsoft C# Coding Conventions
- Use PascalCase for public members
- Use camelCase for private members
- Add XML documentation comments

**Example:**

```csharp
/// <summary>
/// Computes the cotangent weight for an edge.
/// </summary>
/// <returns>The cotangent weight.</returns>
public double Cotan()
{
    // Implementation
}
```

### Python Style

- Follow PEP 8
- Use type hints
- Write docstrings

**Example:**

```python
def compute_curvature(mesh: Mesh) -> np.ndarray:
    """
    Compute discrete Gaussian curvature.
    
    Args:
        mesh: Input triangular mesh
        
    Returns:
        Array of curvature values per vertex
    """
    # Implementation
```

## Commit Guidelines

### Commit Message Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation
- `style`: Formatting
- `refactor`: Code restructuring
- `test`: Adding tests
- `chore`: Maintenance

**Examples:**

```
feat(cpp): Add conformal parameterization algorithm

Implements spectral conformal parameterization using eigenvalue
decomposition of the Laplacian.

Closes #42
```

```
docs(assignments): Add A4 implementation guide

Detailed guide for Assignment 4 with code examples and
testing strategies.
```

## Pull Request Process

1. **Create Feature Branch**
   ```bash
   git checkout -b feature/your-feature
   ```

2. **Make Changes**
   - Write code
   - Add tests
   - Update documentation

3. **Test Locally**
   ```bash
   # C++
   cd build && ctest
   
   # C#
   dotnet test
   ```

4. **Commit Changes**
   ```bash
   git add .
   git commit -m "feat(scope): description"
   ```

5. **Push to Fork**
   ```bash
   git push origin feature/your-feature
   ```

6. **Create Pull Request**
   - Go to GitHub
   - Click "New Pull Request"
   - Fill out template
   - Request review

## Pull Request Template

```markdown
## Description

Brief description of changes.

## Type of Change

- [ ] Bug fix
- [ ] New feature
- [ ] Documentation
- [ ] Refactoring

## Testing

- [ ] Unit tests added/updated
- [ ] Manual testing performed
- [ ] All tests pass

## Checklist

- [ ] Code follows style guidelines
- [ ] Comments added to complex code
- [ ] Documentation updated
- [ ] No new warnings introduced

## Related Issues

Closes #XX
```

## Code Review Process

### Reviewers Check For:

1. **Correctness**
   - Algorithm implemented correctly
   - Edge cases handled
   - No memory leaks (C++)

2. **Code Quality**
   - Readable and maintainable
   - Well-commented
   - Follows style guide

3. **Testing**
   - Adequate test coverage
   - Tests pass
   - Examples work

4. **Documentation**
   - API documented
   - Usage examples provided
   - README updated if needed

## Algorithm Implementation Guidelines

### Before Implementing

1. Read course notes chapter
2. Understand mathematical theory
3. Review existing implementations
4. Plan data structures
5. Write pseudocode

### Implementation Steps

1. **Define Interface**
   ```cpp
   class NewAlgorithm {
   public:
       static ResultType compute(const Mesh& mesh, Parameters params);
   };
   ```

2. **Implement Core Logic**
   - Follow course notes formulas
   - Handle edge cases
   - Add assertions

3. **Add Tests**
   ```cpp
   TEST(NewAlgorithm, BasicTest) {
       // Test on simple mesh
   }
   ```

4. **Document**
   - Add header comments
   - Explain parameters
   - Provide usage example

### Algorithm Checklist

- [ ] Mathematical correctness verified
- [ ] Edge cases handled (boundaries, degenerate triangles)
- [ ] Numerical stability considered
- [ ] Performance optimized (if needed)
- [ ] Unit tests added
- [ ] Documentation written
- [ ] Example usage provided

## Documentation Guidelines

### Chapter Summaries

- **Structure**: Overview, Key Concepts, Formulas, Implementation
- **Length**: 500-1000 words
- **Format**: Markdown with LaTeX math
- **Location**: `docs/chapters/`

### Assignment Guides

- **Structure**: Objective, Tasks, Theory, Implementation, Testing
- **Include**: Code examples, common issues, validation
- **Format**: Markdown
- **Location**: `docs/assignments/`

### Formula Reference

- **Use LaTeX**: Properly formatted equations
- **Include**: Description, chapter reference
- **Cross-reference**: Link to implementations

## Testing Guidelines

### Unit Tests

**C++ (Google Test):**
```cpp
TEST(MeshTest, EulerCharacteristic) {
    Mesh mesh;
    // Build tetrahedron
    EXPECT_EQ(mesh.eulerCharacteristic(), 2);
}
```

**C# (xUnit):**
```csharp
[Fact]
public void TestEulerCharacteristic()
{
    var mesh = new Mesh();
    // Build tetrahedron
    Assert.Equal(2, mesh.EulerCharacteristic());
}
```

### Validation Tests

- Test on known meshes (tetrahedron, cube, sphere)
- Verify mathematical properties (Gauss-Bonnet, symmetry)
- Check convergence to smooth case

## Questions?

Feel free to:
- Open an issue for discussion
- Ask in pull request comments
- Contact [@rhughes42](https://github.com/rhughes42)

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

Thank you for contributing to DDG Course Companion!
