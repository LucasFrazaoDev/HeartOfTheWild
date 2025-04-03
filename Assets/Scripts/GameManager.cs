using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject m_player;

    public void ActivatePlayer()
    {
        m_player.SetActive(true);
    }
}
