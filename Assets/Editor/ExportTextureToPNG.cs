using UnityEngine;
using UnityEditor;
using System.IO;

public class ExportTextureToPNG : MonoBehaviour
{
    [MenuItem("Tools/Export Selected Texture(s) to PNG")]
    public static void ExportSelectedTexturesToPNG()
    {
        foreach (var obj in Selection.objects)
        {
            if (obj is Texture2D texture)
            {
                ExportTexture(texture);
            }
            else
            {
                Debug.LogWarning($"Selected object '{obj.name}' is not a Texture2D.");
            }
        }
    }

    private static void ExportTexture(Texture2D texture)
    {
        string path = EditorUtility.SaveFilePanel("Save PNG", "", texture.name + ".png", "png");
        if (string.IsNullOrEmpty(path)) return;

        RenderTexture rt = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(texture, rt);
        RenderTexture.active = rt;

        Texture2D readableTex = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
        readableTex.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        readableTex.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        byte[] pngBytes = readableTex.EncodeToPNG();
        File.WriteAllBytes(path, pngBytes);

        Debug.Log($"Texture '{texture.name}' saved as PNG to: {path} ({pngBytes.Length / 1024} KB)");
    }
}
