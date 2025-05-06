using UnityEngine;
using FMODUnity;

public class MenuMusicPlayer : MonoBehaviour
{
    [SerializeField] private EventReference menuMusicEvent;

    private FMOD.Studio.EventInstance menuMusicInstance;

    void Start()
    {
        // Cria a instância do evento de música
        menuMusicInstance = RuntimeManager.CreateInstance(menuMusicEvent);

        // Inicia a música
        menuMusicInstance.start();
    }

    void OnDestroy()
    {
        // Para e libera a música quando o objeto for destruído
        menuMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        menuMusicInstance.release();
    }
}