using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AssetDatabaseHelper
{
    public static void CreateOrUpdateAsset<T>(ref T asset, string path)
        where T: Object
    {
        T existingAsset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (existingAsset == null)
        {
            AssetDatabase.CreateAsset(asset, path);
            return;
        }
        
        if (existingAsset is Mesh existingMesh)
        {
            // meshes seem to cause crashes when edited via CopySerialized(), even when doing the internet's suggestion of calling Clear() first.
            // maybe there's a way to do that successfully? for now, manual mesh-copy. i didn't wanna go to heaven anyway
            
            Mesh meshToCopy = asset as Mesh;
            CopyMesh(meshToCopy, existingMesh);
        }
        else
        {
            // anything other than a mesh: much easier
            EditorUtility.CopySerialized(asset, existingAsset);
        }
        
        EditorUtility.SetDirty(existingAsset);
        asset = existingAsset;
    }
    
    public static void CopyMesh(Mesh sourceMesh, Mesh destinationMesh)
    {
        destinationMesh.Clear();
        destinationMesh.indexFormat = sourceMesh.indexFormat;
        destinationMesh.subMeshCount = sourceMesh.subMeshCount;
            
        destinationMesh.vertices = sourceMesh.vertices;
        destinationMesh.colors = sourceMesh.colors;
        destinationMesh.normals = sourceMesh.normals;
        destinationMesh.tangents = sourceMesh.tangents;
        destinationMesh.uv = sourceMesh.uv;
        destinationMesh.uv2 = sourceMesh.uv2;
        destinationMesh.uv3 = sourceMesh.uv3;
        destinationMesh.uv4 = sourceMesh.uv4;
        destinationMesh.uv5 = sourceMesh.uv5;
        destinationMesh.uv6 = sourceMesh.uv6;
        destinationMesh.uv7 = sourceMesh.uv7;
        destinationMesh.uv8 = sourceMesh.uv8;
        destinationMesh.name = sourceMesh.name;
        destinationMesh.boneWeights = sourceMesh.boneWeights;
        destinationMesh.bindposes = sourceMesh.bindposes;
            
        List<int> triangles = new List<int>();
        for (int i=0; i<sourceMesh.subMeshCount; i++)
        {
            sourceMesh.GetTriangles(triangles, i);
            destinationMesh.SetTriangles(triangles, i);
        }
    }
}
