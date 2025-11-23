#!/usr/bin/env python3
"""
Simple mesh visualization utility.

Usage:
    python visualize_mesh.py vertices.txt faces.txt
"""

import sys
import numpy as np
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D

def load_mesh(vertices_file, faces_file):
    """Load mesh from text files."""
    V = np.loadtxt(vertices_file)
    F = np.loadtxt(faces_file, dtype=int)
    return V, F

def compute_statistics(V, F):
    """Compute and print mesh statistics."""
    n_vertices = V.shape[0]
    n_faces = F.shape[0]
    n_edges = n_faces + n_vertices - 2
    euler = n_vertices - n_edges + n_faces
    
    print(f"Mesh Statistics:")
    print(f"  Vertices: {n_vertices}")
    print(f"  Edges: {n_edges}")
    print(f"  Faces: {n_faces}")
    print(f"  Euler characteristic: {euler}")
    print(f"  Genus: {(2 - euler) / 2}")
    
    # Bounding box
    min_coords = V.min(axis=0)
    max_coords = V.max(axis=0)
    print(f"  Bounding box: [{min_coords[0]:.2f}, {max_coords[0]:.2f}] x "
          f"[{min_coords[1]:.2f}, {max_coords[1]:.2f}] x "
          f"[{min_coords[2]:.2f}, {max_coords[2]:.2f}]")

def visualize_mesh(V, F, curvature=None):
    """Visualize mesh with optional curvature coloring."""
    fig = plt.figure(figsize=(10, 10))
    ax = fig.add_subplot(111, projection='3d')
    
    if curvature is not None:
        # Color by curvature
        colors = curvature
        surf = ax.plot_trisurf(V[:,0], V[:,1], V[:,2], 
                               triangles=F,
                               cmap='seismic',
                               vmin=-1, vmax=1,
                               alpha=0.8)
        fig.colorbar(surf, label='Curvature')
    else:
        # Simple wireframe
        ax.plot_trisurf(V[:,0], V[:,1], V[:,2], 
                       triangles=F,
                       color='lightblue',
                       edgecolor='black',
                       linewidth=0.5,
                       alpha=0.8)
    
    ax.set_xlabel('X')
    ax.set_ylabel('Y')
    ax.set_zlabel('Z')
    ax.set_title('Mesh Visualization')
    
    # Equal aspect ratio
    max_range = np.array([V[:,0].max()-V[:,0].min(),
                         V[:,1].max()-V[:,1].min(),
                         V[:,2].max()-V[:,2].min()]).max() / 2.0
    mid_x = (V[:,0].max()+V[:,0].min()) * 0.5
    mid_y = (V[:,1].max()+V[:,1].min()) * 0.5
    mid_z = (V[:,2].max()+V[:,2].min()) * 0.5
    ax.set_xlim(mid_x - max_range, mid_x + max_range)
    ax.set_ylim(mid_y - max_range, mid_y + max_range)
    ax.set_zlim(mid_z - max_range, mid_z + max_range)
    
    plt.show()

if __name__ == '__main__':
    if len(sys.argv) < 3:
        print("Usage: python visualize_mesh.py vertices.txt faces.txt [curvature.txt]")
        sys.exit(1)
    
    vertices_file = sys.argv[1]
    faces_file = sys.argv[2]
    
    V, F = load_mesh(vertices_file, faces_file)
    compute_statistics(V, F)
    
    curvature = None
    if len(sys.argv) > 3:
        curvature = np.loadtxt(sys.argv[3])
    
    visualize_mesh(V, F, curvature)
