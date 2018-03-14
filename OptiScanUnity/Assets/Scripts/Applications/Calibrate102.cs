using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// using NUnit.Framework;
// using Python.Runtime;
using System;

// using IronPython;
// using IronPython.Modules;
//using UnityEditor;

public class Calibrate102 : MonoBehaviour {

    // E: matrix btw cmr and TrackCmr
    // T: TrackCmr
    // p': virtual reference floating points
    // p3: positions from optitrack
    // (ET)^(-1)P'=P3
    // E(TP3) = P'
    // then we feed TP3 and P' sets to python script and figure out the E

    Matrix4x4 mtxEye;
    //Matrix4x4 mtxTrackEye;

    public Transform trackerEye;
    public Transform[] trackerPoints;
    public Transform[] floatingPoints;

    public TextMesh vLog;

    // Use this for initialization
    void Start () {
        mtxEye = Matrix4x4.identity;

//         Matrix4x4 r1 = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.Euler(0, 90, 0), new Vector3(1, 1, 1));
//         Matrix4x4 t1 = Matrix4x4.TRS(new Vector3(1, 0, 0), Quaternion.identity, Vector3.one);
//         Matrix4x4 res = r1 * t1;
//         Matrix4x4 res2 = t1 * r1;
    }
	
	// Update is called once per frame
	void Update () {
        // later we applied camera's p and q every frame
    }

    public void OnTouch()
    {
        vLog.text = "TOUCHED";
        

        //
        if(trackerEye.gameObject.GetComponent<TrackableComponent>().Tracked &&
            trackerPoints[0].parent.gameObject.GetComponent<TrackableMarker>().Tracked)
                GetComponent<SyncMtx102>().SetHost(true);
    }

    public Vector3[] GetPointPairs()
    {
        // E(TP3) = P'
        Vector3[] pp = new Vector3[8];
        // T: TrackCmr
        // p3: positions from optitrack
        Matrix4x4 mtrackerEye = Matrix4x4.TRS(trackerEye.position, trackerEye.rotation, Vector3.one);
        for (int i = 0; i < 4; i++)
        {
            Vector3 v = mtrackerEye.MultiplyPoint(trackerPoints[i].position);
            pp[i] = v;
        }
        // p': virtual reference floating points
        for (int i = 4; i < 8; i++)
        {
            pp[i] = floatingPoints[i - 4].position;
        }
        return pp;
    }

    

//     [MenuItem("Python/IronPython")]
//     public static void ScriptTest()
//     {
//         // create the engine  
//         var ScriptEngine = IronPython.Hosting.Python.CreateEngine();
// 
// //         string prefix = "D:\\Program Files\\Python27\\lib, D:\\Program Files\\Python27, D:\\Program Files\\Python27\\lib\\site-packages";
// //         string exec = "";
// //         string version = "";
// //         IronPython.Hosting.Python.SetHostVariables(ScriptEngine, prefix, exec, version);
//         var sp = ScriptEngine.GetSearchPaths();
//         sp.Add("D:\\Program Files\\Python27\\lib");
//         sp.Add("D:\\Program Files\\Python27");
//         sp.Add("D:\\Program Files\\Python27\\lib\\site-package");
//         ScriptEngine.SetSearchPaths(sp);
// 
//         sp = ScriptEngine.GetSearchPaths();
//         foreach (var s in sp)
//             Debug.Log(s);
// 
// 
//         // and the scope (ie, the python namespace)  
//         var ScriptScope = ScriptEngine.CreateScope();
//         // execute a string in the interpreter and grab the variable  
//         string example = "output = 'hello world ???'";
//         var ScriptSource = ScriptEngine.CreateScriptSourceFromString(example);
//         ScriptSource.Execute(ScriptScope);
//         string came_from_script = ScriptScope.GetVariable<string>("output");
//         // Should be what we put into 'output' in the script.  
//         Debug.Log(came_from_script);
// 
// 
//         var ScriptScope2 = ScriptEngine.Runtime.ExecuteFile("D:\\Projects\\ScanAR\\OptiScanUnity\\SVD\\test.py");
// //         var ScriptSource2 = ScriptEngine.CreateScriptSourceFromFile("D:\\Projects\\ScanAR\\OptiScanUnity\\SVD\\test.py");
// //         //var ScriptScope2 = IronPython.Hosting.Python.ImportModule(ScriptEngine, "numpy");
// //         var ScriptScope2 = ScriptEngine.CreateScope();
// //         ScriptSource2.Execute(ScriptScope2);
//         came_from_script = ScriptScope2.GetVariable<string>("R");
// 
// 
//         // Should be what we put into 'output' in the script.  
//         Debug.Log(came_from_script);
//     }
// 
//     [MenuItem("Python/PythonScope")]
//     public static void PythonNetTest()
//     {
//         SetUp();
//         using (Py.GIL())
//         {
//             dynamic np = Py.Import("numpy");
//             Debug.Log("after import numpy in test");
// 
//             var locals = new PyDict();
//             locals.SetItem("a", new PyInt(10));
//             var c = locals.GetItem("a");
//             PythonEngine.Exec("a = 2 + 1", null, locals.Handle);
//             Debug.Log("after exec in test");
//             //var c = locals.GetItem("a");
//             //Debug.Log(c);
// 
// //             dynamic ps = Py.CreateScope("test");
// //             ps.Set("bb", 100); //declare a global variable
// //             ps.Set("cc", 10); //declare a local variable
// //             ps.Exec("aa = bb + cc + 3");
// //             Debug.Log("after exec ps in test");
//             //             Debug.Log(np.cos(np.pi * 2));
//             // 
//             //             dynamic sin = np.sin;
//             //             Debug.Log(sin(5));
//             // 
//             //             double c = np.cos(5) + sin(5);
//             //             Debug.Log(c);
//             // 
//             //             dynamic a = np.array(new List<float> { 1, 2, 3 });
//             //             Debug.Log(a.dtype);
//             // 
//             //             dynamic b = np.array(new List<float> { 6, 5, 4 }, dtype: np.int32);
//             //             Debug.Log(b.dtype);
//             // 
//             //             Debug.Log(a * b);
// 
//             //ps.Dispose();
//             //ps = null;
//         }
//         Dispose();
//     }
// 
//     //[OneTimeSetUp]
//     public static void SetUp()
//     {
//         PythonEngine.Initialize();
//     }
// 
//     //[OneTimeTearDown]
//     public static void Dispose()
//     {
//         PythonEngine.Shutdown();
//     }
// 
//     [MenuItem("Python/PythonNetExample")]
//     public static void TestReadme()
//     {
//         SetUp();
//         dynamic np;
//         try
//         {
//             np = Py.Import("numpy");
//             Debug.Log("After imported numpy in readme");
//         }
//         catch (PythonException)
//         {
//             Debug.Log("Numpy or dependency not installed");
//             return;
//         }
// 
//         var locals = new PyDict();
//         
// 
//         PythonEngine.Exec("c = 2 + 1", null, locals.Handle);
//         var c = locals.GetItem("c");
//         Debug.Log(c);
// 
//         //Assert.AreEqual("1.0", np.cos(np.pi * 2).ToString());
// 
//         //var sin = np.sin;
//         //StringAssert.StartsWith("-0.95892", sin(5).ToString());
// 
//         //double c = np.cos(5) + np.sin(5);
//         //Assert.AreEqual(-0.675262, c, 0.01);
// 
//         //dynamic a = np.array(new List<float> { 1, 2, 3 });
//         //Assert.AreEqual("float64", a.dtype.ToString());
// 
//         //dynamic b = np.array(new List<float> { 6, 5, 4 }, Py.kw("dtype", np.int32));
//         //Assert.AreEqual("int32", b.dtype.ToString());
// 
//         //Assert.AreEqual("[ 6. 10. 12.]", (a * b).ToString().Replace("  ", " "));
// 
//         Dispose();
//     }
}
