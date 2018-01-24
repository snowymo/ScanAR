using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// information from david scanner
public class DavidMesh  {
    // matrixes
    public Matrix4x4 transformUncalibrated;
    public Matrix4x4 transformCalibrated;
    public Matrix4x4 scannerControllerMatrix;
    public Matrix4x4 transformScannerControllerToDavidSystem;

    public Vector3[] vertices;
    public int[] faces;

    int status; // 0 as start, 2 as ready, 3 as already sent

    public DavidMesh()
    {
        status = 0;
    }

    public void setMesh(Vector3[] v, int[] f)
    {
        vertices = v;
        faces = f;
        status++;
    }

    public void setMatrix(float[] matrixs)
    {
        // split float[64] to four matrix
        for(int i = 0; i < 4; i ++)
            for(int j = 0; j < 4; j ++)
                transformUncalibrated[i + j * 4] = matrixs[i * 4 + j];

        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                transformCalibrated[i + j * 4] = matrixs[16 + i * 4 + j];

        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                scannerControllerMatrix[i + j * 4] = matrixs[32 + i * 4 + j];

        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                transformScannerControllerToDavidSystem[i + j * 4] = matrixs[48 + i * 4 + j];

        status++;
    }

    public void setMatrix4x4(int index, float[] matrix)
    {
        if (index == 0)
            for(int i = 0; i < 16; i++)
                transformUncalibrated[i] = matrix[i];
        if (index == 1)
            for (int i = 0; i < 16; i++)
                transformCalibrated[i] = matrix[i];
        if (index == 2)
            for (int i = 0; i < 16; i++)
                scannerControllerMatrix[i] = matrix[i];
        if (index == 3)
            for (int i = 0; i < 16; i++)
                transformScannerControllerToDavidSystem[i] = matrix[i];
        if (index == 3)
            status++;
    }
    public void sendMesh()
    {
        status++;
    }

    public int getStatus()
    {
        return status;
    }
	
}
