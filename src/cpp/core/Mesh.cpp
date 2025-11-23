#include "Mesh.h"
#include <map>

void Mesh::build(const Eigen::MatrixXd& V, const Eigen::MatrixXi& F) {
    vertices.clear(); edges.clear(); faces.clear(); halfedges.clear();
    
    for (int i = 0; i < V.rows(); i++) {
        vertices.push_back(std::make_unique<Vertex>(V.row(i)));
    }
    
    std::map<std::pair<int,int>, HalfEdge*> heMap;
    
    for (int i = 0; i < F.rows(); i++) {
        auto face = std::make_unique<Face>();
        std::vector<HalfEdge*> faceHEs;
        
        for (int j = 0; j < 3; j++) {
            auto he = std::make_unique<HalfEdge>();
            he->vertex = vertices[F(i, (j+1)%3)].get();
            he->face = face.get();
            faceHEs.push_back(he.get());
            heMap[{F(i,j), F(i,(j+1)%3)}] = he.get();
            halfedges.push_back(std::move(he));
        }
        
        for (int j = 0; j < 3; j++) faceHEs[j]->next = faceHEs[(j+1)%3];
        face->halfedge = faceHEs[0];
        faces.push_back(std::move(face));
    }
    
    for (auto& [key, he] : heMap) {
        auto twin = heMap.find({key.second, key.first});
        if (twin != heMap.end()) {
            he->twin = twin->second;
            if (!he->edge) {
                auto edge = std::make_unique<Edge>();
                edge->halfedge = he;
                he->edge = edge.get();
                twin->second->edge = edge.get();
                edges.push_back(std::move(edge));
            }
        }
    }
    
    for (auto& he : halfedges) {
        if (!he->vertex->halfedge) he->vertex->halfedge = he.get();
    }
    reindex();
}

void Mesh::reindex() {
    for (size_t i = 0; i < vertices.size(); i++) vertices[i]->index = i;
    for (size_t i = 0; i < edges.size(); i++) edges[i]->index = i;
    for (size_t i = 0; i < faces.size(); i++) faces[i]->index = i;
}

Eigen::MatrixXd Mesh::vertexPositions() const {
    Eigen::MatrixXd V(numVertices(), 3);
    for (size_t i = 0; i < numVertices(); i++) V.row(i) = vertices[i]->position;
    return V;
}

void Mesh::setVertexPositions(const Eigen::MatrixXd& V) {
    for (size_t i = 0; i < numVertices(); i++) vertices[i]->position = V.row(i);
}

void Mesh::center() {
    Eigen::Vector3d c = Eigen::Vector3d::Zero();
    for (const auto& v : vertices) c += v->position;
    c /= numVertices();
    for (auto& v : vertices) v->position -= c;
}

void Mesh::normalize() {
    center();
    double maxD = 0;
    for (const auto& v : vertices) maxD = std::max(maxD, v->position.norm());
    if (maxD > 0) for (auto& v : vertices) v->position /= maxD;
}
