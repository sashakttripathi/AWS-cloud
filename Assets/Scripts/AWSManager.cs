using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class AWSManager : MonoBehaviour
{
    struct image
    {
        public byte[] data;
    }
    // Android Fix
    public void UsedOnlyForAOTCodeGeneration()
    {
        AndroidJavaObject jo = new AndroidJavaObject("andriod.os.Message");
        int valueString = jo.Get<int>("what");
    }


    // Cognito setup
    private string IdentityPoolId = "ap-south-1:85fe3eff-2e69-4e81-9fdc-673bd0f422d4";
    private string Region = RegionEndpoint.APSouth1.SystemName;

    private static AWSManager _instance;
    public static AWSManager Instance
    {
        get
        {
            if(_instance == null)
            {
                Debug.Log("Instance is null");
            }
            return _instance;
        }
    }

    private CognitoAWSCredentials _credentials;
    public CognitoAWSCredentials Credentials
    {
        get
        {
            if(_credentials == null)
            {
                _credentials = new CognitoAWSCredentials(IdentityPoolId, RegionEndpoint.APSouth1);
            }
            return _credentials;
        }
    }

    private string identityId;

    // S3 setup
    private string _S3Region = RegionEndpoint.APSouth1.SystemName;
    public RegionEndpoint S3Region
    {
        get
        {
            return RegionEndpoint.GetBySystemName(_S3Region);
        }
    }

    private AmazonS3Client _S3Client;
    public AmazonS3Client S3Client
    {
        get
        {
            if(_S3Client == null)
            {
                _S3Client = new AmazonS3Client(new CognitoAWSCredentials(IdentityPoolId, RegionEndpoint.APSouth1), S3Region);
            }
            return _S3Client;
        }
    }

    [SerializeField] private string bucketName;

    // initialization
    private void Start()
    {
        _instance = this;

        UnityInitializer.AttachToGameObject(this.gameObject);
        try
        {
            AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
            Debug.Log("connected");
        }
        catch(Exception e)
        {
            Debug.Log("Unable to connect to AWS server");
        }
        Credentials.GetIdentityIdAsync(delegate (AmazonCognitoIdentityResult<string> result)
        {
            if(result.Exception != null)
            {
                Debug.Log(result.Exception);
            }
            else
            {
                identityId = result.Response;
                Debug.Log("result recorded");
            }
        });
    }

    public void Upload(string key)
    {
        if (key == "")
            return;

        Texture2D copyTex = FindObjectOfType<Write>().CallDuplicateTexture();
        image _image; 
        _image.data = copyTex.EncodeToPNG();

        MemoryStream ms = new MemoryStream(_image.data);

        PostObjectRequest request = new PostObjectRequest()
        {
            Bucket = bucketName,
            Key = key,
            InputStream = ms,
            CannedACL = S3CannedACL.Private,
            Region = S3Region
        };

        S3Client.PostObjectAsync(request, (response) =>
        {
            if(response.Exception == null)
            {
                Debug.Log("Uploaded");
            }
            else
            {
                Debug.Log(response.Exception);
            }
        });
    }

    byte[] downloadedImageData;

    public void Download(string key)
    {
        if (key == "")
            return;

        image _image;
        Debug.Log("Download Started");
        var request = new ListObjectsRequest()
        {
            BucketName = bucketName
        };

        S3Client.ListObjectsAsync(request, (response) =>
        {
            S3Client.GetObjectAsync(bucketName, key, (responseObj) =>
            {
                if(responseObj.Exception == null)
                {
                    Debug.Log("Noi Exceeption");
                    byte[] data = null;
                    using (StreamReader sr = new StreamReader(responseObj.Response.ResponseStream))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            var buffer = new byte[50000];                                               // 50 MB files max
                            var bytesRead = default(int);
                            while((bytesRead = sr.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, bytesRead);
                            }
                            data = ms.ToArray();
                        }
                    }
                    _image.data = data;
                    //using(MemoryStream ms = new MemoryStream(data))
                    //{
                    //    BinaryFormatter bf = new BinaryFormatter();
                    //    _image.data = ms.ToArray();
                    //    // downloadedImageData = _image.data;
                    //}
                    Debug.Log("Just before saving");
                    SaveDownloadedPic(_image.data, key);

                }
                else
                {
                    Debug.Log("Exception: " + responseObj.Exception);
                }
            });
        });
    }

    void SaveDownloadedPic(byte[] downloadedImageData, string key)
    {
        Debug.Log("saving pic");
        File.WriteAllBytes(Application.persistentDataPath+ "/" + key, downloadedImageData);
        Debug.Log(Application.persistentDataPath);
    }

}
