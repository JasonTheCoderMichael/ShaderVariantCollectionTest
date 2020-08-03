using UnityEngine;

public class KeywordAndMultiCompile : MonoBehaviour
{
    public Transform sf_trans;
    
    private ShaderVariantCollection svc;
    
    private Material m_material;
    private string m_curMC = "";
    private string m_curSF = "";
    private AssetBundle m_ab;
    
    private void OnEnable()
    {
        m_material = GetMaterial(sf_trans);
    }

    private void OnGUI()
    {
        // Multi Compile //
        if (GUI.Button(new Rect(0, 0, 100, 50), "MC_Red"))
        {
            Shader.DisableKeyword(m_curMC);
            m_curMC = "_MC_RED";
            Shader.EnableKeyword(m_curMC);
        }
        
        if (GUI.Button(new Rect(100, 0, 100, 50), "MC_Green"))
        {
            Shader.DisableKeyword(m_curMC);
            m_curMC = "_MC_GREEN";
            Shader.EnableKeyword(m_curMC);
        }
        
        if (GUI.Button(new Rect(200, 0, 100, 50), "MC_Blue"))
        {
            Shader.DisableKeyword(m_curMC);
            m_curMC = "_MC_BLUE";
            Shader.EnableKeyword(m_curMC);
        }
        
        if (GUI.Button(new Rect(300, 0, 100, 50), "MC_Disable"))
        {
            Shader.DisableKeyword(m_curMC);
        }

        // Shader Feature //
        if (GUI.Button(new Rect(0, 100, 100, 50), "SF_Red"))
        {
            SetShaderFeature(m_curSF, false);
            m_curSF = "_SF_RED";
            SetShaderFeature(m_curSF, true);
        }
        
        if (GUI.Button(new Rect(100, 100, 100, 50), "SF_Green"))
        {
            SetShaderFeature(m_curSF, false);
            m_curSF = "_SF_GREEN";
            SetShaderFeature(m_curSF, true);
        }
        
        if (GUI.Button(new Rect(200, 100, 100, 50), "SF_Blue"))
        {
            SetShaderFeature(m_curSF, false);
            m_curSF = "_SF_BLUE";
            SetShaderFeature(m_curSF, true);
        }
        
        if (GUI.Button(new Rect(300, 100, 100, 50), "SF_Disable"))
        {
            SetShaderFeature(m_curSF, false);
            // m_curSF = "_";strp
            // SetShaderFeature(m_curSF, true);
        }

        if (GUI.Button(new Rect(0, 200, 100, 50), "Load SVC"))
        {
            LoadAB();
            LoadSVC("AthenaCurrentSVC.shadervariants");
            // ChangeMaterial();
            ChangeShader();
        }
    }

    private void SetShaderFeature(string keyword, bool state)
    {
        if (state)
        {
            Shader.EnableKeyword(keyword);   
        }
        else
        {
            Shader.DisableKeyword(keyword);
        }
        return;
        
        Material material = GetMaterial(sf_trans);
        if (material != null)
        {
            if (state)
            {
                material.EnableKeyword(keyword);    
            }
            else
            {
                material.DisableKeyword(keyword);
            }
        }
    }

    private Material GetMaterial(Transform trans)
    {
        if (trans == null)
        {
            return null;
        }
        
        Renderer render = sf_trans.GetComponent<Renderer>();
        if (render == null)
        {
            return null;
        }
        
        return render.material;
    }
    
    private void LoadAB()
    {
        m_ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/svc.ab");
    }

    private void LoadSVC(string svcFileName)
    {
        if (m_ab != null)
        {
            ShaderVariantCollection svc = m_ab.LoadAsset<ShaderVariantCollection>(svcFileName);
            if (svc != null)
            {
                svc.WarmUp();
            }   
        }
    }

    private Shader LoadShader()
    {
        if (m_ab == null)
        {
            return null;
        }
        
        return m_ab.LoadAsset<Shader>("Test_shaderfeature.shader");
    }
    
    private void ChangeShader()
    {
        GameObject cube = GameObject.Find("Cube2");
        if (cube != null)
        {
            Renderer render = cube.GetComponent<Renderer>();
            if (render != null)
            {
                Shader shader = LoadShader();
                render.material.shader = shader;
                
                // Material material = new Material(shader);
                // render.material = material;
            }
        }
    }
}
