/*
* 2018-06-06 ZoJet
*/


using System.Collections;
using System.IO;
using UnityEngine;

/// <summary>
/// Get Texture2D/Sprite form a local picture 
/// </summary>
public class Picture {
    /// <summary>
    /// Copy picture bytes
    /// </summary>
    /// <param name="_path"></param>
    /// <returns></returns>
    public static byte[] GetBytes(string _path) {
        FileStream fileStream = new FileStream(_path, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);

        byte[] bytes = new byte[fileStream.Length];
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;
        return bytes;
    }

    /// <summary>
    /// Get a local picture as a Texture2D
    /// </summary>
    /// <param name="_path"></param>
    /// <param name="_width"></param>
    /// <param name="_height"></param>
    /// <returns></returns>
    public static Texture2D GetTexture(string _path, int _width, int _height) {
        byte[] bytes;
        try {
            bytes = GetBytes(_path);
        } catch (System.Exception e) {
            Debug.Log(e.InnerException);
            return null;
        }
        if (bytes == null || bytes.Length == 0) {
            return null;
        }

        Texture2D tex = new Texture2D(_width, _height);
        tex.LoadImage(bytes);

        string[] strs = _path.Split('/');
        tex.name = strs[strs.Length - 1].Split('.')[0];
        return tex;
    }

    /// <summary>
    /// Get a local picture as Sprite. Example : SetPictureToSprite("Assets/StreamingAssets/Bg01.jpg",1920,1080);
    /// </summary>
    /// <param name="_path"></param>
    /// <param name="_width"></param>
    /// <param name="_height"></param>
    /// <returns></returns>
    public static Sprite GetSprite(string _path, int _width, int _height) {
        if (_path == null) {
            return null;
        }
        Texture2D tex = GetTexture(_path, _width, _height);
        if (tex == null) {
            return null;
        }
        Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        sp.name = tex.name;
        return sp;
    }

    /// <summary>
    /// Get Texture2Ds according to picture's path and type
    /// </summary>
    /// <param name="_path"></param>
    /// <param name="_pictureType"></param>
    /// <param name="_width"></param>
    /// <param name="_height"></param>
    /// <returns></returns>
    public static Texture2D[] GetTextures(string _path, string _pictureType, int _width, int _height) {
        string[] files = Directory.GetFiles(_path, _pictureType, SearchOption.AllDirectories);
        Texture2D[] texs = new Texture2D[files.Length];
        for (int i = 0; i < files.Length; i++) {
            texs[i] = GetTexture(files[i], _width, _height);
            string[] strs = files[i].Split('\\');//Sprite file path
            texs[i].name = strs[strs.Length - 1].Split('.')[0];//Get tex name
        }
        return texs;
    }

    /// <summary>
    /// Get Sprites according to picture's path and type. 
    /// Example : SetPicturesToSprites("Assets/Resources/", "*.jpg",1920, 1080);
    /// </summary>
    /// <param name="_path"></param>
    /// <param name="_pictureType"></param>
    /// <param name="_width"></param>
    /// <param name="_height"></param>
    /// <returns></returns>
    public static Sprite[] GetSprites(string _path, string _pictureType, int _width, int _height) {
        if (_path == null) {
            return null;
        }
        Texture2D[] texs = GetTextures(_path, _pictureType, _width, _height);
        if (texs == null) {
            return null;
        }
        Sprite[] sprites = new Sprite[texs.Length];
        for (int i = 0; i < texs.Length; i++) {
            sprites[i] = Sprite.Create(texs[i], new Rect(0, 0, texs[i].width, texs[i].height), new Vector2(0.5f, 0.5f));
            sprites[i].name = texs[i].name;
        }
        return sprites;
    }

    /// <summary>
    /// Get Texture2D by IOStream
    /// </summary>
    /// <param name="_path"></param>
    /// <returns></returns>
    public static Texture2D GetTextureByIO(string _path) {
        FileStream fileStream = new FileStream(_path, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        byte[] bytes = new byte[fileStream.Length];
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        Texture2D tex = new Texture2D(1, 1);
        tex.LoadImage(bytes);
        return tex;
    }

    /// <summary>
    /// Get Texture2D by WWW
    ///     path = "http://pic.nipic.com/2007-11-09/2007119122519868_2.jpg";
    ///     path = "file://" + Application.dataPath + "/huichen.jpg";           //未成功
    /// </summary>
    /// <param name="_path"></param>
    /// <param name="_tex"></param>
    /// <returns></returns>
    public static IEnumerator GetTextureByWWW(string _path, Texture2D _tex) {
        WWW www = new WWW(_path);
        yield return www;
        if (string.IsNullOrEmpty(www.error) == false) {
            Debug.Log(www.error);
        } else {
            _tex = www.texture;
        }
    }
}


