using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ImageUploader m_imageUploader;
    public ImageCapture m_imageCapture;
    public ImageProcessor m_imageProcessor;

    public bool m_On_Capture = false;
    public bool m_On_Upload = false;


    public void StartImageUploader(RenderTexture sourceTexture, RenderTexture targetTexture){
        StartCoroutine(
            m_imageUploader.UploadImage(
                m_imageUploader.RenderTexture_To_Byte(sourceTexture),
                targetTexture
            )
        );
        m_captureCounter = 0;
            Debug.Log("Image Upload!");
    }

    
    public float m_captureCounter = 0 ;
    public float m_captureDelay = 1;

    public void CaptureAndSaveImage(RenderTexture renderTexture, int fileNum, string filepath){
        m_imageCapture.CaptureAndSaveImage(renderTexture, fileNum, filepath);
        m_captureCounter = 0;
    }
    void Update(){
        m_captureCounter += Time.deltaTime;
    }

    public void Passing_Texture(RenderTexture source, RenderTexture target)
    {
        m_imageProcessor.Render_To_Render_Passing(source, target);
    }
    public void Shading_Texture(RenderTexture source, RenderTexture target, Material material){
        m_imageProcessor.ImgShader(source, target, material);
    }
    /*====================================================S====================*/
    /*========================================================================*/
    /*========================================================================*/
    /*========================================================================*/
    /*========================================================================*/
    public static GameManager instance = null;
    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    /*========================================================================*/
    /*========================================================================*/
    /*========================================================================*/
    /*========================================================================*/
    /*========================================================================*/
}
