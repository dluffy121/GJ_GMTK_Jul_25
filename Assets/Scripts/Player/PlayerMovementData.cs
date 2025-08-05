using UnityEngine;

namespace GJ_GMTK_Jul_2025
{
    [CreateAssetMenu()]
    public class PlayerMovementData : ScriptableObject
    {
        [SerializeField] public float BaseLoopSpeed = 10;
        [SerializeField] public float LoopOffset = 1;
        [SerializeField] public float LoopDirFlipCooldown = 2;
        [SerializeField] public float LoopCorrectionMultiplier = 0.01f;
        [SerializeField] public float LoopCorrectionMultiplierFadeSpeed = 0.01f;

        [SerializeField] public float BaseMoveSpeed = 10;
        [SerializeField] public float MoveCooldown = .5f;
    }
}