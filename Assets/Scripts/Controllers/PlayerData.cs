using UnityEngine;

namespace GJ_GMTK_Jul_2025
{
    [CreateAssetMenu()]
    public class PlayerData : ScriptableObject
    {
        [SerializeField] public float BaseLoopSpeed = 10;
        [SerializeField] public float LoopOffset = 1;

        [SerializeField] public float BaseMoveSpeed = 10;
    }
}