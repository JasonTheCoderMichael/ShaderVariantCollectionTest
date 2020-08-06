using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class SVCHelper
{
    [MenuItem("MJ/BuildAB_SVC")]
    private static void BuildAB_SVC()
    {
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath,
            BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows);
    }
    
    [MenuItem("MJ/CreateAthenaSVC")]
    private static void CreateAthenaSVC()
    {
        string svcFileName = "AthenaCurrentSVC.shadervariants";
        SaveCurrentSVC(svcFileName);

        List<VariantInfo> variantInfos = GetVariantInfo(svcFileName);
            
        string svcFileName2 = "AthenaSVC.shadervariants";
        CreateSVC(svcFileName2, variantInfos);
    }
    
    private static void CreateSVC(string svcFileName, List<VariantInfo> variantInfos)
    {
        if (variantInfos == null || variantInfos.Count == 0)
        {
            return;
        }

        ShaderVariantCollection athenaSVC = new ShaderVariantCollection();
        for (int i = 0; i < variantInfos.Count; i++)
        {
            VariantInfo info = variantInfos[i];
            if (info == null || info.keywords == null)
            {
                continue;
            }

            for (int j = 0; j < info.keywords.Length; j++)
            {
                ShaderVariantCollection.ShaderVariant variant = new ShaderVariantCollection.ShaderVariant()
                {
                    shader = info.shader, keywords = new string[1]{info.keywords[j]}, passType = info.passType[j],
                };
                athenaSVC.Add(variant);
            }
        }
        
        // CreateAsset 的path参数要传 “Assets/Dir/file.postfix"  形式 // 
        // 不能用 Application.dataPath + "/dIR/FILE.postfix" 形式 // 
        
    #if UNITY_EDITOR
        AssetDatabase.CreateAsset(athenaSVC, "Assets/KeywordAndMultiCompile/" + svcFileName);
        AssetDatabase.Refresh();
    #endif
    }

    private static ShaderGUIDInfo[] GetAthenaShaderGUIdList(Shader[] shaders)
    {
        if (shaders == null || shaders.Length == 0)
        {
            return null;
        }

        int shaderCount = shaders.Length;
        ShaderGUIDInfo[] result = new ShaderGUIDInfo[shaderCount];
        for (int i = 0; i < shaderCount; i++)
        {
            Shader shader = shaders[i];

            ShaderGUIDInfo guidInfo = new ShaderGUIDInfo();
            guidInfo.shader = shader;
            // guidInfo.guid = GetShaderGUID(shader);
            guidInfo.guid = GetShaderGUID2(shader);
            result[i] = guidInfo;
        }

        return result;
    }
    
    // 获取shader的guid //
    private static string GetShaderGUID(Shader shader)
    {
        if (shader == null)
        {
            return "";
        }
        
    #if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(shader);
    #else
        string path = "";
    #endif
        
        string metaPath = GetMetaFilePath(path);
        return GetGUIDFromMetaFile(metaPath);
    }

    private static string GetShaderGUID2(Shader shader)
    {
        string path = AssetDatabase.GetAssetPath(shader);
        return AssetDatabase.AssetPathToGUID(path);
    }

    private static string GetMetaFilePath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }
        
        return path + ".meta";
    }

    private static string GetGUIDFromMetaFile(string metaFilePath)
    {
        if (string.IsNullOrEmpty(metaFilePath))
        {
            return "";
        }

        if (!File.Exists(metaFilePath))
        {
            return "";
        }

        string guid = "";
        string[] lines = File.ReadAllLines(metaFilePath);
        if (lines.Length > 1)
        {
            string line = lines[1];
            if (line.Contains("guid") && line.Contains(":"))
            {
                string[] kvp = line.Split(':');
                if (kvp.Length > 1)
                {
                    guid = kvp[1];
                }
            }
        }

        return guid;
    }

    private static List<VariantInfo> GetVariantInfo(string svcFileName)
    {
        if (string.IsNullOrEmpty(svcFileName))
        {
            return null;
        }
        
        Shader shader1 = Shader.Find("MJ/Test_shaderfeature");

        Shader[] athenaShaders = new Shader[] { shader1 };
        ShaderGUIDInfo[] shaderGUIDInfos = GetAthenaShaderGUIdList(athenaShaders);

        if (shaderGUIDInfos == null || shaderGUIDInfos.Length == 0)
        {
            return null;
        }
        
        string svcFilePath = Application.dataPath + "/KeywordAndMultiCompile/" + svcFileName;
        if (!File.Exists(svcFilePath))
        {
            return null;
        }
        
        return StripRedundantVariant(svcFilePath, shaderGUIDInfos);
    }

    private static List<VariantInfo> StripRedundantVariant(string svcFilePath, ShaderGUIDInfo[] shaderGUIDs)
    {
        if (string.IsNullOrEmpty(svcFilePath) || shaderGUIDs == null || shaderGUIDs.Length == 0)
        {
            return null;
        }

        if (!File.Exists(svcFilePath))
        {
            return null;
        }
        
        string[] lines = File.ReadAllLines(svcFilePath);
        List<VariantInfo> result = new List<VariantInfo>();
        
        // temp 变量 //
        VariantInfo tempInfo = null;
        List<string> tempKeywordList = null;
        List<PassType> tempPassTypeList = null;
        
        // 逻辑: 遇到 guid 行时，把tempKeywordList和tempPassTypeList转换成数组，设置给tempInfo, 再把tempInfo添加到result列表里 //
        int skipLineNum = 0;
        bool parseLine = false;
        for (int i = 0; i < lines.Length; i++)
        {
            if (skipLineNum > 0)
            {
                skipLineNum--;
                parseLine = true;
                continue;
            }
            
            string line = lines[i];

            int guidInfoIndex = LineContainsGUID(line, shaderGUIDs);
            bool findGUID = (guidInfoIndex != -1);
            if (findGUID)
            {
                skipLineNum = 2;
                parseLine = false;

                SetKeywordAndPasstype(tempInfo, tempKeywordList, tempPassTypeList);

                tempInfo = new VariantInfo(){};
                tempInfo.shader = shaderGUIDs[guidInfoIndex].shader;
                tempKeywordList = new List<string>();
                tempPassTypeList = new List<PassType>();
                result.Add(tempInfo);
                
                continue;
            }

            if (parseLine)
            {
                if (line.Contains("keywords") && tempInfo != null && tempKeywordList != null)
                {
                    tempKeywordList.Add(GetKeywords(line));
                }
                else if (line.Contains("passType") && tempInfo != null && tempPassTypeList != null)
                {
                    tempPassTypeList.Add(GetPassType(line));
                }
            }
            
            bool islastLine = (i == lines.Length - 1);
            if (islastLine)
            {
                SetKeywordAndPasstype(tempInfo, tempKeywordList, tempPassTypeList);
                break;
            }
        }
        
        return result;
    }

    // 把list转为array设置给variantinfo对象 //
    private static void SetKeywordAndPasstype(VariantInfo tempInfo, List<string> tempKeywordList, List<PassType> tempPassTypeList)
    {
        if (tempInfo != null)
        {
            if (tempKeywordList != null && tempPassTypeList != null && 
                tempKeywordList.Count == tempPassTypeList.Count)
            {
                tempInfo.keywords = tempKeywordList.ToArray();
                tempInfo.passType = tempPassTypeList.ToArray();
            }
        }
    }

    // 返回值是对象在ShaderGUIDInfo数组中的索引, 没找到时返回-1 //
    private static int LineContainsGUID(string line, ShaderGUIDInfo[] guidInfos)
    {
        if (string.IsNullOrEmpty(line) || guidInfos == null || guidInfos.Length == 0)
        {
            return -1;
        }
        
        for (int i = 0; i < guidInfos.Length; i++)
        {
            ShaderGUIDInfo guidInfo = guidInfos[i]; 
            if (guidInfo != null && line.Contains(guidInfo.guid))
            {
                return i;
            }
        }

        return -1;
    }

    private static PassType GetPassType(string line)
    {
        if (string.IsNullOrEmpty(line) || !line.Contains(":"))
        {
            return PassType.Normal;
        }

        string[] kvp = line.Split(':');
        if (kvp.Length != 2)
        {
            return PassType.Normal;
        }

        int passType = 0;
        if (int.TryParse(kvp[1], out passType))
        {
            return (PassType)passType;
        }
        return PassType.Normal;
    }

    private static string GetKeywords(string line)
    {
        if (string.IsNullOrEmpty(line) || !line.Contains(":"))
        {
            return "";
        }

        string[] kvp = line.Split(':');
        if (kvp.Length != 2)
        {
            return "";
        }

        return kvp[1];
    }
    
    private static void SaveCurrentSVC(string svcName)
    {
        // EditorSceneManager.OpenScene(sceneName);
        
        MethodInfo clearSVCFunction = typeof(ShaderUtil).GetMethod("ClearCurrentShaderVariantCollection", BindingFlags.NonPublic | BindingFlags.Static);
        if (clearSVCFunction != null)
        {
            clearSVCFunction.Invoke(null, null);   
        }

        MethodInfo saveSVCFunction = typeof(ShaderUtil).GetMethod("SaveCurrentShaderVariantCollection", BindingFlags.NonPublic | BindingFlags.Static);

        if (saveSVCFunction != null)
        {
            object[] args = new object[]
            {
                "Assets/KeywordAndMultiCompile/" + svcName,
            };
            
            saveSVCFunction.Invoke(null, args);
        }
        AssetDatabase.Refresh();
    } 
    
    // 1个shader对应多个passtype和多个keyword字符串 //
    public class VariantInfo
    {
        public Shader shader;
        public PassType[] passType;
        public string[] keywords;
        
        public VariantInfo()
        {
            this.shader = null;
            passType = null;
            keywords = null;
        }
    }

    public class ShaderGUIDInfo
    {
        public Shader shader;
        public string guid;
    }
}
