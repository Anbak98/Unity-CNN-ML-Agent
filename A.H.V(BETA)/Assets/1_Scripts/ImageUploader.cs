using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class ImageUploader : MonoBehaviour
{
    private string serverUrl = "http://localhost:5555/upload"; // 서버 URL을 적절히 변경하세요.

    // 이미지를 업로드하는 함수: 이미지 데이터를 매개변수에
    public IEnumerator UploadImage(byte[] imageData, RenderTexture renderTexture)
    {
        // int byteLength = imageData.Length;
		// Debug.Log("바이트 배열의 크기: " + byteLength);

        WWWForm form = new WWWForm();
        form.AddBinaryData("image", imageData, "image.png", "image/png");

        // POST 요청 보내기
        using (UnityWebRequest www = UnityWebRequest.Post(serverUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                // Debug.LogError("Image upload failed: " + www.error);
            }
            else
            {
                Debug.Log("Image upload successful");
                byte[] imageBytes = www.downloadHandler.data;
                ConvertPNGToRenderTexture(imageBytes, renderTexture);
            }
        }
    }
    public byte[] RenderTexture_To_Byte(RenderTexture _renderImg){
        // Ensure the RenderTexture is active or set it as the active RenderTexture

        Texture2D texture2D = new Texture2D(_renderImg.width, _renderImg.height, TextureFormat.ARGB32, false);
        RenderTexture.active = _renderImg;
        texture2D.ReadPixels(new Rect(0, 0, _renderImg.width, _renderImg.height), 0, 0);
        texture2D.Apply();

        return texture2D.EncodeToPNG();
    }
     public void ConvertPNGToRenderTexture(byte[] pngData, RenderTexture renderTexture)
    {
        // Load the PNG data into a Texture2D
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        if (texture.LoadImage(pngData))
        {
            Graphics.Blit(texture, renderTexture);
            Destroy(texture);
        }
        else
        {
            Debug.LogError("Failed to load PNG data as a texture.");
        }
    }
}
