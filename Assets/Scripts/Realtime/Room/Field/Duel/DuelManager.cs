using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.IO;
using System;
using IronPython.Hosting;

namespace Room
{
    public class DuelManager : MonoBehaviour
    {
        private List<string> libs;
        private string code;

        private void Awake()
        {
            var url = Application.dataPath + "/Scripts/Realtime/Room/Field/Duel/comp_bind.py";
            code = File.ReadAllText(url);
            Debug.Log(code);
            
            libs = new()
            {
                Application.dataPath + "/ThridParty/Python/Lib"
            };
        }

        [EasyButtons.Button]
        private void SumFirst()
        {
            var engine = Python.CreateEngine();
            var scope = engine.CreateScope();

            var searchPaths = engine.GetSearchPaths();
            foreach (var lib in libs)
            {
                searchPaths.Add(lib);
            }
            engine.SetSearchPaths(searchPaths);

            UniTask.RunOnThreadPool(() =>
            {
                var source = engine.CreateScriptSourceFromString(code);
                source.Execute(scope);
                
                var func = scope.GetVariable<Func<object>>("comper");
            }, false).Forget();
        }
        
        private void SumSecond()
        {

        }

        private void SumThird()
        {

        }
    }
}
