using UnityEngine;
using UnityEditor;

public class BatchShaderReplacer : EditorWindow
{
    [MenuItem("Tools/批量替换Shader为Standard")]
    public static void ReplaceShaders()
    {
        string[] materialGuids = AssetDatabase.FindAssets("t:Material");

        int changedCount = 0;
        foreach (string guid in materialGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (mat != null && mat.shader.name != "Standard")
            {
                mat.shader = Shader.Find("Standard");
                changedCount++;
                Debug.Log($"更换材质：{mat.name} -> Standard");
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"✔️ 已完成：共修改 {changedCount} 个材质球的 Shader 为 Standard。");
    }
}
