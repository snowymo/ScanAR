using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TestLoader : MonoBehaviour
{
    public string fileName;
    public bool isAbsolute;
    public string filePath;

    public string currentScan, currentScanOSR;

    void preloadTest()
    {
        if (!isAbsolute)
            filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        IntPtr plyIntPtr = PlyLoaderDll.LoadPly(filePath);

        Mesh mesh = new Mesh();
        mesh.vertices = PlyLoaderDll.GetVertices(plyIntPtr);
        //mesh.uv = PlyLoaderDll.GetUvs(plyIntPtr);
        //mesh.normals = PlyLoaderDll.GetNormals(plyIntPtr);
        //mesh.colors32 = PlyLoaderDll.GetColors(plyIntPtr);
        mesh.SetIndices(PlyLoaderDll.GetIndexs(plyIntPtr), MeshTopology.Triangles, 0, true);
        mesh.name = "mesh";

        GameObject go = new GameObject();
        go.name = "meshNew";
        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = mesh;
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("UCLA Game Lab/Wireframe/Double-Sided"));
        string textureName = PlyLoaderDll.GetTextureName(plyIntPtr);
        if (textureName == null)
            textureName = "mesh_UVtexture.jpg";
        if (textureName != null && textureName.Length > 0)
        {
            string texturePath = "file://" + System.IO.Path.Combine(Application.streamingAssetsPath, textureName);
            WWW www = new WWW(texturePath);
            while (!www.isDone)
            {
            }
            mr.material.mainTexture = www.texture;
        }

        PlyLoaderDll.UnLoadPly(plyIntPtr);
    }

    void loadPLYwithPtr(IntPtr p)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = PlyLoaderDll.GetVertices(p);
        //mesh.uv = PlyLoaderDll.GetUvs(plyIntPtr);
        //mesh.normals = PlyLoaderDll.GetNormals(plyIntPtr);
        //mesh.colors32 = PlyLoaderDll.GetColors(plyIntPtr);
        mesh.SetIndices(PlyLoaderDll.GetIndexs(p), MeshTopology.Triangles, 0, true);
        mesh.name = "mesh";

        GameObject go = new GameObject();
        go.name = "meshNew";
        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = mesh;
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("UCLA Game Lab/Wireframe/Double-Sided"));
    }

    // Use this for initialization
    void Start()
    {
        
    }
    bool mutex = true;
    int i = 0;
    void Update()
    {
        StartCoroutine(sleep500ms());

        // load file until success and then remove
        if (mutex && File.Exists(currentScan))
        {
            print("testLoader" + mutex + i++ + Time.time);
            StartCoroutine(sleep500ms());
            mutex = false;
            IntPtr plyIntPtr = PlyLoaderDll.LoadPly(currentScan);
            //loadPLYwithPtr(plyIntPtr);
            PlyLoaderDll.UnLoadPly(plyIntPtr);
            //File.Delete(currentScan);
            File.Move(currentScan, currentScanOSR);
            mutex = true;
        }
    }

    IEnumerator sleep500ms()
    {
        //print(Time.time);
        yield return new WaitForSeconds(0.5f);
        //print(Time.time);
    }
}