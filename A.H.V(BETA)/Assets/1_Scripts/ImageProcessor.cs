using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageProcessor : MonoBehaviour
{
    public void Render_To_Render_Passing(RenderTexture source, RenderTexture target){
        Graphics.Blit(source, target);
    }
    public void ImgShader(RenderTexture source, RenderTexture target, Material material){
        if(material == null){
            Graphics.Blit(source, target);
            return;
        }
        Graphics.Blit(source, target, material);
    }
}
