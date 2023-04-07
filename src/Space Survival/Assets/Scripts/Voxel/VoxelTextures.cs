using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTextures : MonoBehaviour
{
    #region Singleton
    public static VoxelTextures Instance;
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    #endregion

    [System.Serializable]
    class TextureType
    {
        public string name;
        public Vector2 coords;
    }

    [SerializeField] Material referencePalette;

    [Tooltip("The size of the texture map")]
    [SerializeField] Vector2 textureSize;
    [Tooltip("Set textures with name keys and vector2 coords valus of the sub textures")]
    [SerializeField] List<TextureType> textures;

    /// <summary>
    /// Returns the uv coords for the given texture
    /// </summary>
    /// <param name="_textureName">The name of the texture to get</param>
    /// <returns>Vector2 coords of the sub-texture in the palette texture</returns>
    public Vector2 GetTexture(string _textureName)
    {
        foreach (TextureType _texture in textures) {
            if (_texture.name == _textureName) {
                //Small offset prevent flickering between colours
                return (_texture.coords / textureSize) + new Vector2(0.01f, 0.01f);
            }
        }
        return Vector2.zero;
    }
}
