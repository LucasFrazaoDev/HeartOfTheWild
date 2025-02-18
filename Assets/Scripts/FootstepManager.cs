using System.Collections.Generic;
using UnityEngine;

public class FootstepManager : MonoBehaviour
{
    private AudioSource m_audioSource;

    [Header("Footsteps settings")]
    [SerializeField] private float m_distanceToGround = 0.1f;
    [SerializeField] private LayerMask m_layerMask;
    [SerializeField] private float m_maxHeightDifference = 0.5f;
    [SerializeField] private float m_thresholdValue = 0.1f;
    [Space(10)]

    [Header("Audio lists for footsteps")]
    [SerializeField] private List<AudioClip> m_floorFootsteps = new List<AudioClip>();
    [SerializeField] private List<AudioClip> m_rampFootsteps = new List<AudioClip>();
    [Space(10)]

    [Header("Feet bones")]
    [SerializeField] private Transform m_leftFootBone;
    [SerializeField] private Transform m_rightFootBone;

    private bool m_isLeftFootDown;
    private bool m_isRightFootDown;
    private SurfaceType m_surfaceType;

    private const string k_floorTagName = "Floor";
    private const string k_rampTagName = "Ramp";

    private enum SurfaceType
    {
        Floor,
        Ramp
    }

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        CheckFootstepState(m_leftFootBone, ref m_isLeftFootDown);
        CheckFootstepState(m_rightFootBone, ref m_isRightFootDown);
    }

    private void CheckFootstepState(Transform foot, ref bool isFootDown)
    {
        if (foot == null)
            return;

        RaycastHit hit;
        Ray ray = new Ray(foot.position, Vector3.down);

        if (Physics.Raycast(ray, out hit, m_distanceToGround, m_layerMask))
        {
            float heightDifference = Mathf.Abs(hit.point.y - foot.position.y);
            float dynamicThreshold = Mathf.Clamp01(heightDifference / m_maxHeightDifference);

            if (!isFootDown && dynamicThreshold < m_thresholdValue)
            {
                isFootDown = true;
                DetermineSurfaceType(hit.collider.tag);
                PlayFootstepSound();
            }
        }
        else
            isFootDown = false;
    }

    private void DetermineSurfaceType(string tag)
    {
        switch (tag)
        {
            case k_floorTagName:
                m_surfaceType = SurfaceType.Floor;
                break;
            case k_rampTagName:
                m_surfaceType = SurfaceType.Ramp;
                break;
        }
    }

    private void PlayFootstepSound()
    {
        List<AudioClip> footstepSounds = GetFootstepSounds();

        if (footstepSounds.Count > 0)
        {
            AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Count)];
            m_audioSource.PlayOneShot(clip);
        }
    }

    private List<AudioClip> GetFootstepSounds()
    {
        switch (m_surfaceType)
        {
            case SurfaceType.Floor:
                return m_floorFootsteps;
            case SurfaceType.Ramp:
                return m_rampFootsteps;
            default:
                return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (m_leftFootBone != null && m_rightFootBone != null)
        {
            DrawFootRay(m_leftFootBone);
            DrawFootRay(m_rightFootBone);
        }
    }

    private void DrawFootRay(Transform foot)
    {
        if (foot == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(foot.position, Vector3.down * m_distanceToGround);
    }
}
