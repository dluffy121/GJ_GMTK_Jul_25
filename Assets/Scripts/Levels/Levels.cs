using UnityEngine;

public class Levels : MonoBehaviour
{
    [SerializeField]
    private Transform m_playerPos;

    public Transform playerPos => m_playerPos;

}
