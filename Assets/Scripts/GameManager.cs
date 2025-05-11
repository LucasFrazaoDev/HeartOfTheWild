using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject m_player;
    [SerializeField] private PauseManager m_gameUI;

    public void ActivatePlayer()
    {
        m_player.SetActive(true);
    }

    public void CutsceneTransition()
    {
        m_gameUI.OnCutsceneFinished();
    }
}
