#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class RenderCameraToPNG : MonoBehaviour
{

    Camera cam;

    public void RenderToImage(Vector2Int res, string name)
    {
        StartCoroutine(renderToFile(res, name));
    }

    IEnumerator renderToFile(Vector2Int res, string filepath)
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("No camera attached!"); yield break;
        }
        yield return new WaitForEndOfFrame();

        Screen.SetResolution(res.x, res.y, false);
        RenderTexture rt = new RenderTexture(res.x, res.y, 16, RenderTextureFormat.ARGB32);
        rt.Create();

        cam.targetTexture = rt;

        Texture2D tex = new Texture2D(cam.activeTexture.width, cam.activeTexture.height, TextureFormat.ARGB32, false);
        RenderTexture.active = cam.activeTexture;
        yield return new WaitForEndOfFrame();
        tex.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        byte[] img_bytes = tex.EncodeToPNG();

        File.WriteAllBytes(filepath, img_bytes);

        cam.targetTexture = null;

        Destroy(tex);
        Destroy(rt);

        yield break;
    }
}

[CustomEditor(typeof(RenderCameraToPNG))]
public class RenderCameraToPNG_Editor : Editor
{

    string imageName = "tmp/def.png";
    Vector2Int imageResolution = new Vector2Int(512, 512);

    public override void OnInspectorGUI()
    {
        RenderCameraToPNG _target = (RenderCameraToPNG) target;
        imageName = EditorGUILayout.TextField("File name", imageName);
        imageResolution = EditorGUILayout.Vector2IntField("Resolution", imageResolution);

        if(GUILayout.Button("RENDER"))
        {
            if (imageResolution.magnitude >= 360 && imageName.Length >= 3)
            {
                if (Application.isPlaying)
                    _target.RenderToImage(imageResolution, imageName);
                else
                    Debug.LogError("You must press PLAY before rendering!");
            }
            else
                Debug.LogError("Please make sure that you select a resolution greater than 256x256, " +
                    "and a image length greather than 2 characters.");
        }

    }
}

#endif