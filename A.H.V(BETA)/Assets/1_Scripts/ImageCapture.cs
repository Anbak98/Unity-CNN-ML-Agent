using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class ImageCapture : MonoBehaviour
{
    public byte[] m_imgbyte;
    
    public void CaptureAndSaveImage(RenderTexture renderTexture, int fileNum, string filepath)
    {
        /*
        // Ensure the captureCamera is not null
        if (captureCamera == null)
        {
            Debug.LogError("Capture Camera is not assigned!");
            return;
        }

        // Create a RenderTexture to capture the camera's output
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        captureCamera.targetTexture = renderTexture;

        // Render the camera's view into the RenderTexture
        captureCamera.Render();
        */

        // Create a Texture2D and read the pixels from the RenderTexture 
        // Debug.Log(renderTexture.width);
        Texture2D screenShot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenShot.Apply();

        //
        m_imgbyte = screenShot.EncodeToPNG();


        
        System.IO.File.WriteAllBytes(filepath + fileNum + ".png", m_imgbyte);

        /* Clean up resources
        RenderTexture.active = null;
        captureCamera.targetTexture = null;
        //Destroy(renderTexture);*/

        // Debug.Log("Image captured and saved to " + filepath);
        
    }
}