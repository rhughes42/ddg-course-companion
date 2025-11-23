#include "Vertex.h"
#include "HalfEdge.h"
#include "Face.h"

std::vector<Vertex*> Vertex::star() const {
    std::vector<Vertex*> neighbors;
    if (!halfedge) return neighbors;
    
    HalfEdge* he = halfedge;
    do {
        neighbors.push_back(he->next->vertex);
        he = he->twin->next;
    } while (he != halfedge);
    
    return neighbors;
}

int Vertex::degree() const {
    if (!halfedge) return 0;
    
    int count = 0;
    HalfEdge* he = halfedge;
    do {
        count++;
        he = he->twin->next;
    } while (he != halfedge);
    
    return count;
}

bool Vertex::isBoundary() const {
    if (!halfedge) return true;
    
    HalfEdge* he = halfedge;
    do {
        if (!he->face) return true;
        he = he->twin->next;
    } while (he != halfedge);
    
    return false;
}

std::vector<HalfEdge*> Vertex::outgoingHalfEdges() const {
    std::vector<HalfEdge*> halfedges;
    if (!halfedge) return halfedges;
    
    HalfEdge* he = halfedge;
    do {
        halfedges.push_back(he);
        he = he->twin->next;
    } while (he != halfedge);
    
    return halfedges;
}

std::vector<Face*> Vertex::adjacentFaces() const {
    std::vector<Face*> faces;
    if (!halfedge) return faces;
    
    HalfEdge* he = halfedge;
    do {
        if (he->face) {
            faces.push_back(he->face);
        }
        he = he->twin->next;
    } while (he != halfedge);
    
    return faces;
}
