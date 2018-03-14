using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IronPython;
using IronPython.Modules;
using UnityEditor;

public class Calibrate102 : MonoBehaviour {

    // E: matrix btw cmr and TrackCmr
    // T: TrackCmr
    // p': virtual reference floating points
    // p3: positions from optitrack
    // (ET)-1P'=P3
    // E(TP3) = P'
    // then we feed TP3 and P' sets to python script and figure out the E

    Matrix4x4 mtxEye;
    Matrix4x4 mtxTrackEye;

    public Transform trackerEye;
    public Transform[] trackerPoints;
    public Transform[] floatingPoints;

    public TextMesh vLog;

    // Use this for initialization
    void Start () {
        mtxEye = Matrix4x4.identity;

    }
	
	// Update is called once per frame
	void Update () {
        // later we applied camera's p and q every frame
    }

    public void OnTouch()
    {
        vLog.text = "TOUCHED";
        

        //
        GetComponent<SyncMtx>().SetHost(true);
    }

    [MenuItem("Python/HelloWorld")]
    public static void ScriptTest()
    {
        // create the engine  
        var ScriptEngine = IronPython.Hosting.Python.CreateEngine();

//         string prefix = "D:\\Program Files\\Python27\\lib, D:\\Program Files\\Python27, D:\\Program Files\\Python27\\lib\\site-packages";
//         string exec = "";
//         string version = "";
//         IronPython.Hosting.Python.SetHostVariables(ScriptEngine, prefix, exec, version);
        var sp = ScriptEngine.GetSearchPaths();
        sp.Add("D:\\Program Files\\Python27\\lib");
        sp.Add("D:\\Program Files\\Python27");
        sp.Add("D:\\Program Files\\Python27\\lib\\site-package");
        ScriptEngine.SetSearchPaths(sp);

        sp = ScriptEngine.GetSearchPaths();
        foreach (var s in sp)
            Debug.Log(s);


        // and the scope (ie, the python namespace)  
        var ScriptScope = ScriptEngine.CreateScope();
        // execute a string in the interpreter and grab the variable  
        string example = "output = 'hello world ???'";
        var ScriptSource = ScriptEngine.CreateScriptSourceFromString(example);
        ScriptSource.Execute(ScriptScope);
        string came_from_script = ScriptScope.GetVariable<string>("output");
        // Should be what we put into 'output' in the script.  
        Debug.Log(came_from_script);


        var ScriptScope2 = ScriptEngine.Runtime.ExecuteFile("D:\\Projects\\ScanAR\\OptiScanUnity\\SVD\\test.py");
//         var ScriptSource2 = ScriptEngine.CreateScriptSourceFromFile("D:\\Projects\\ScanAR\\OptiScanUnity\\SVD\\test.py");
//         //var ScriptScope2 = IronPython.Hosting.Python.ImportModule(ScriptEngine, "numpy");
//         var ScriptScope2 = ScriptEngine.CreateScope();
//         ScriptSource2.Execute(ScriptScope2);
        came_from_script = ScriptScope2.GetVariable<string>("R");


        // Should be what we put into 'output' in the script.  
        Debug.Log(came_from_script);
    }
}
