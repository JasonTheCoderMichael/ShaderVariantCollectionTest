using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class KWAndMCShaderGUI : ShaderGUI
{
    private readonly string[] SF_Group1 = new string[]
    {
        "_", "_SF_RED", "_SF_GREEN", "_SF_BLUE"
    };
    
    private readonly string[] SF_Group2 = new string[]
    {
        "_", "TEST_KW1", "TEST_KW2", "TEST_KW3"
    };

    private string m_lastKW = "";
    private string m_lastKW2 = "";
    private int m_index = 0;
    private int m_index2 = 0;
    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        Material targetMat = materialEditor.target as Material;

        for (int i = 0; i < SF_Group1.Length; i++)
        {
            if (targetMat.IsKeywordEnabled(SF_Group1[i]))
            {
                m_index = i;
                m_lastKW = SF_Group1[m_index];
            }
        }
        
        EditorGUI.BeginChangeCheck();
        m_index = EditorGUILayout.Popup("SF_Group1 ", m_index, SF_Group1);
        if (EditorGUI.EndChangeCheck())
        {
            if (m_index >= 0 && m_index < SF_Group1.Length)
            {
                targetMat.DisableKeyword(m_lastKW);
                
                string keyword = SF_Group1[m_index];
                m_lastKW = keyword;
                targetMat.EnableKeyword(keyword);
            }
        }
        
        for (int i = 0; i < SF_Group2.Length; i++)
        {
            if (targetMat.IsKeywordEnabled(SF_Group2[i]))
            {
                m_index2 = i;
                m_lastKW2 = SF_Group2[m_index2];
            }
        }
        
        EditorGUI.BeginChangeCheck();
        m_index2 = EditorGUILayout.Popup("Group2 ", m_index2, SF_Group2);
        if (EditorGUI.EndChangeCheck())
        {
            if (m_index2 >= 0 && m_index2 < SF_Group2.Length)
            {
                targetMat.DisableKeyword(m_lastKW2);
                
                string keyword = SF_Group2[m_index2];
                m_lastKW2 = keyword;
                targetMat.EnableKeyword(keyword);
            }
        }
    }
}
