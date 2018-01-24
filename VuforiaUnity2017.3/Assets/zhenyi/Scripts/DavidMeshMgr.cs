using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DavidMeshMgr  {

    List<DavidMesh> meshes;

	public DavidMeshMgr()
    {
        meshes = new List<DavidMesh>();
    }

    public void generateNew()
    {
        meshes.Add(new DavidMesh());
    }

    public void setMesh(Vector3[] v, int[] f)
    {
        meshes[meshes.Count - 1].setMesh(v, f);
    }

    public void setMatrix(float[] matrixs)
    {
        meshes[meshes.Count - 1].setMatrix(matrixs);
    }

    public void setMatrix4x4(int index, float[] matrixs)
    {
        meshes[meshes.Count - 1].setMatrix4x4(index, matrixs);
        
    }

    public DavidMesh getLatest()
    {
        return meshes[meshes.Count - 1];
    }

    public bool readyForSend()
    {
        if (meshes[meshes.Count - 1].getStatus() == 2)
        {
            meshes[meshes.Count - 1].sendMesh();
            return true;
        }
        else
            return false;
    }
}
