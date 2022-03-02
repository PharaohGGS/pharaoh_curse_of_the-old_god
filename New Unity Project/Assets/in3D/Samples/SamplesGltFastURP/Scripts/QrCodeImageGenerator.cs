using System;
using in3D.AvatarsSDK.Configurations;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

[RequireComponent(typeof(RawImage))]
public class QrCodeImageGenerator : MonoBehaviour
{
    [SerializeField] private AvatarsServer server;
    [SerializeField] private UserConfiguration user;
    
    private RawImage _image;
    
    // Start is called before the first frame update
    void Start()
    {
        // Generating uuid
        Guid guid = Guid.NewGuid();
        
        // Change user config
        user.ConnectWithQr(guid.ToString());
        
        // Create qr text for authorization
        string qrText =$"{guid.ToString()}-{server.Vendor}-vto";
        
        // Display qr
        _image = GetComponent<RawImage>();
        _image.texture = GenerateQrTexture(qrText, 256);
    }
    
    private static Color32[] EncodeQr(string textForEncoding, int width, int height) {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }

    public static Texture2D GenerateQrTexture(string text, int size)
    {
        var encoded = new Texture2D (size, size);
        var color32 = EncodeQr(text, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();
        return encoded;
    }

}
