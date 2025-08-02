using UnityEngine;
using UnityEditor;
using System.IO;

public class GenerateNoiseTexture : EditorWindow
{
    int width = 256;
    int height = 256;
    float scale = 10f;
    float tiling = 1f;
    Vector2 tileOffset = Vector2.zero;

    bool useRandomSeed = true;
    int seed = 0;

    int octaves = 1;
    float persistence = 0.5f;
    float lacunarity = 2f;

    Gradient colorGradient = new Gradient();

    string fileName = "NoiseTexture.png";

    [MenuItem("Tools/Generate Noise Texture")]
    public static void ShowWindow()
    {
        GetWindow<GenerateNoiseTexture>("Noise Texture Generator");
    }

    void OnEnable()
    {
        // Initialize a default gradient (black→white) if none is set
        if (colorGradient.colorKeys.Length == 0)
        {
            colorGradient = new Gradient();
            colorGradient.colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(Color.black, 0f),
                new GradientColorKey(Color.white, 1f)
            };
        }
    }

    void OnGUI()
    {
        GUILayout.Label("Noise Texture Settings", EditorStyles.boldLabel);

        // Dimensions & scale
        width  = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);
        scale  = EditorGUILayout.FloatField("Scale", scale);
        tiling = EditorGUILayout.FloatField("Tiling", tiling);
        tileOffset = EditorGUILayout.Vector2Field("Tile Offset", tileOffset);

        EditorGUILayout.Space();

        // Seed options
        useRandomSeed = EditorGUILayout.Toggle("Use Random Seed", useRandomSeed);
        if (!useRandomSeed)
        {
            seed = EditorGUILayout.IntField("Seed Value", seed);
        }

        EditorGUILayout.Space();

        // Fractal (octaves) settings
        GUILayout.Label("Fractal Settings", EditorStyles.boldLabel);
        octaves     = EditorGUILayout.IntSlider("Octaves", octaves, 1, 8);
        persistence = EditorGUILayout.Slider("Persistence", persistence, 0f, 1f);
        lacunarity  = EditorGUILayout.Slider("Lacunarity", lacunarity, 1f, 4f);

        EditorGUILayout.Space();

        // Color mapping
        GUILayout.Label("Color Mapping", EditorStyles.boldLabel);
        colorGradient = EditorGUILayout.GradientField("Gradient", colorGradient);

        EditorGUILayout.Space();
        
        // Output
        fileName = EditorGUILayout.TextField("File Name", fileName);

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate and Save"))
        {
            // Validate
            if (width <= 0 || height <= 0)
            {
                EditorUtility.DisplayDialog("Error", "Width and Height must be greater than 0", "OK");
                return;
            }

            // Generate
            Texture2D tex = GeneratePerlinNoiseTexture(width, height);
            // Save
            string path = "Assets/" + fileName;
            SaveTextureAsPNG(tex, path);
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Done", $"Texture saved to {path}", "OK");
        }
    }

    Texture2D GeneratePerlinNoiseTexture(int w, int h)
    {
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGB24, false);

        // Determine seed offsets
        float seedX = useRandomSeed ? Random.Range(0f, 1000f) : seed;
        float seedY = useRandomSeed ? Random.Range(0f, 1000f) : seed * 2f;

        // Precompute normalization factor for octaves
        float amplitude = 1f;
        float maxAmp = 0f;
        for (int i = 0; i < octaves; i++)
        {
            maxAmp += amplitude;
            amplitude *= persistence;
        }

        // Fill pixels
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float xNorm = (x / (float)w) * scale * tiling + tileOffset.x;
                float yNorm = (y / (float)h) * scale * tiling + tileOffset.y;

                float frequency = 1f;
                float amp = 1f;
                float noiseValue = 0f;

                // Sum octaves
                for (int o = 0; o < octaves; o++)
                {
                    float sample = Mathf.PerlinNoise(
                        (xNorm + seedX) * frequency,
                        (yNorm + seedY) * frequency
                    ) * amp;

                    noiseValue += sample;

                    frequency *= lacunarity;
                    amp *= persistence;
                }

                // Normalize to [0,1]
                noiseValue /= maxAmp;

                // Map to color via gradient
                Color col = colorGradient.Evaluate(noiseValue);
                tex.SetPixel(x, y, col);
            }
        }

        tex.Apply();
        return tex;
    }

    void SaveTextureAsPNG(Texture2D texture, string path)
    {
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);
    }
}
