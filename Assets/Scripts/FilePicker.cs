using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class FilePicker : MonoBehaviour
{
    // public Texture2D tex, texCopy;
    byte[] newPicData;
    Texture2D texture, copy;
    //public void ChoosePic()
    //{
    //    string path = EditorUtility.OpenFilePanel("Open File", "", "png");
    //    if(path.Length != 0)
    //    {
    //        var fileContent = File.ReadAllBytes(path);
    //        newPicData = fileContent;
    //        FindObjectOfType<Write>().ManagePic(newPicData);
    //    }
    //}

    public void ChoosePic()
    {
        NativeGallery.Permission permissionS3Course = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path.Length != 0)
            {
                texture = NativeGallery.LoadImageAtPath(path, 5000);
                copy = FindObjectOfType<Write>().CallDuplicateTexture(texture);
                newPicData = copy.EncodeToPNG();
                FindObjectOfType<Write>().ManagePic(newPicData);
            }
        });
    }
}
