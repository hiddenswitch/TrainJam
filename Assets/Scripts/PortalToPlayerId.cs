using UnityEngine;

public class PortalToPlayerId : MonoBehaviour
{
    [SerializeField] private int m_ToPlayerId;

    public int toPlayerId => m_ToPlayerId;
}