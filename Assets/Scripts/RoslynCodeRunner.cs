using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DilmerGames.Core.Singletons;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Linq;
using TMPro;

public class RoslynCodeRunner : Singleton<RoslynCodeRunner>
{
    [SerializeField]
    private string[] namespaces;

    [SerializeField]
    [TextArea(5, 12)]
    private string code;

    [SerializeField]
    [TextArea(5, 12)]
    private string additionalCode;

    [SerializeField]
    private UnityEvent OnRunCodeCompleted;

    [SerializeField]
    private string[] resultVars;

    [SerializeField]
    [TextArea(5, 12)]
    private string resultInfo;



    //why null, only want to pass code through inspector
    public void RunCode(string updatedCode = null)
    {
        Logger.Instance.LogInfo("executing code...");
        updatedCode = string.IsNullOrEmpty(updatedCode) ? null : updatedCode;
        try
        {
            code = $"{(updatedCode ?? code)} {additionalCode}";
            ScriptState<object> result = CSharpScript.RunAsync(code, SetDefaultImports()).Result;

            foreach(string var in resultVars)
            {
                resultInfo += $"{result.GetVariable(var).Name}: {result.GetVariable(var).Value}\n";
            }
            
            OnRunCodeCompleted?.Invoke();
        }
        catch (Exception e)
        {
            Logger.Instance.LogError(e.Message);
        }
    }

    //preset namespace for the code
    private ScriptOptions SetDefaultImports()
    {
        return ScriptOptions.Default
            .WithImports(namespaces.Select(n => n.Replace("using", string.Empty)
            .Trim()))
            .AddReferences(
                typeof(MonoBehaviour).Assembly,
                typeof(Debug).Assembly,
                typeof(TextMeshPro).Assembly,
                typeof(IEnumerator).Assembly,
                typeof(Rigidbody).Assembly,
                typeof(Collider).Assembly,
                typeof(Input).Assembly,
                typeof(OVRInput).Assembly,
                typeof(Animation).Assembly,
                typeof(Light).Assembly,
                typeof(PhysicMaterial).Assembly,
                typeof(Camera).Assembly,
                typeof(TextMesh).Assembly,
                typeof(ParticleSystem).Assembly);

    }
}
