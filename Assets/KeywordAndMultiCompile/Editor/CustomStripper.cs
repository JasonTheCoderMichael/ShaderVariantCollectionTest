using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomStripper : IPreprocessShaders
{
    private List<string> KeyworsToKeep_FP = new List<string>()
        {"_SF_RED", "_SF_GREEN"};
    
    private List<string> KeyworsToKeep_VP = new List<string>()
        {"_SF_BLUE"};
    
    public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
    {
        return;
        
        if (shader.name != "MJ/Test_shaderfeature")
        {
            return;
        }
        
        // 只处理 vertex 和 fragment lit.shad阶段 //
        List<string> kwToKeep = null;
        if (snippet.shaderType == ShaderType.Vertex)
        {
            kwToKeep = KeyworsToKeep_VP;
        }
        else if(snippet.shaderType == ShaderType.Fragment)
        {
            kwToKeep = KeyworsToKeep_FP;
        }
        
        if (kwToKeep == null)
        {
            return;
        }
        
        for (int i = data.Count - 1; i >= 0; i--)
        {
            ShaderCompilerData scd = data[i];

            bool neesStrip = false;

            for (int j = 0; j < kwToKeep.Count; j++)
            {
                ShaderKeyword kw = new ShaderKeyword(kwToKeep[j]);
                if (!scd.shaderKeywordSet.IsEnabled(kw))
                {
                    neesStrip = true;
                    break;
                }
            }

            if (neesStrip)
            {
                data.RemoveAt(i);
            }
        }
    }
    
    public int callbackOrder
    {
        get { return 0; }
    }
}
