using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Write : MonoBehaviour
{
    public Image avatar;

    public Texture2D CallDuplicateTexture()
    {
        return duplicateTexture((Texture2D)avatar.mainTexture);
    }

    public Texture2D CallDuplicateTexture(Texture2D tex)
    {
        return duplicateTexture(tex);
    }

    //duplicating to make the texture readable as a byte array
    private Texture2D duplicateTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
            source.width,
            source.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);
        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }

    // just shows the image in the UI
    public void ManagePic(byte[] newImageData)
    {
        Texture2D text = new Texture2D(0, 0);
        text.LoadImage(newImageData);
        text.Apply();
        Sprite newImage = Sprite.Create(text, new Rect(0.0f, 0.0f, text.width, text.height), new Vector2(0.5f, 0.5f), 100.0f);
        avatar.sprite = newImage;
    }
}
