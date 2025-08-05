using UnityEngine;

public static class GradientExtensions
{
    public static void BlendGradients(this Gradient thisGrad, Gradient gradA, Gradient gradB, float t)
    {
        // Blend color keys
        GradientColorKey[] keysA = gradA.colorKeys;
        GradientColorKey[] keysB = gradB.colorKeys;
        int keyCount = Mathf.Min(keysA.Length, keysB.Length);
        GradientColorKey[] blendedKeys = new GradientColorKey[keyCount];
        for (int i = 0; i < keyCount; i++)
        {
            blendedKeys[i].color = Color.Lerp(keysA[i].color, keysB[i].color, t);
            blendedKeys[i].time = Mathf.Lerp(keysA[i].time, keysB[i].time, t);
        }

        // Blend alpha keys
        GradientAlphaKey[] alphaA = gradA.alphaKeys;
        GradientAlphaKey[] alphaB = gradB.alphaKeys;
        int alphaCount = Mathf.Min(alphaA.Length, alphaB.Length);
        GradientAlphaKey[] blendedAlpha = new GradientAlphaKey[alphaCount];
        for (int i = 0; i < alphaCount; i++)
        {
            blendedAlpha[i].alpha = Mathf.Lerp(alphaA[i].alpha, alphaB[i].alpha, t);
            blendedAlpha[i].time = Mathf.Lerp(alphaA[i].time, alphaB[i].time, t);
        }

        thisGrad.SetKeys(blendedKeys, blendedAlpha);
    }
}