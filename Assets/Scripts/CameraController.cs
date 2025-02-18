using UnityEngine;
using Unity.Cinemachine;

public class ThirdPersonCameraController : MonoBehaviour
{
    [SerializeField] private InputReaderSO m_inputReader;
    [SerializeField] private GameObject m_cinemachineTarget;

    [Header("Mouse settings")]
    [Range(50f, 300f)][SerializeField] private float m_lookSensitivity = 50f;
    [Tooltip("How far in degrees can move the camera up")]
    [SerializeField] private float m_topClamp = 70.0f;
    [Tooltip("How far in degrees can move the camera down")]
    [SerializeField] private float m_bottomClamp = -30.0f;
    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    private float m_targetYaw;
    private float m_targetPitch;

    void Start()
    {
        m_targetYaw = m_cinemachineTarget.transform.rotation.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (m_inputReader == null || m_cinemachineTarget == null)
        {
            Debug.LogError("InputReader or CinemachineTarget not assigned.");
            return;
        }
        CameraRotation();
    }

    private void CameraRotation()
    {
        m_targetYaw += m_inputReader.GetLookVector().x * m_lookSensitivity * Time.unscaledDeltaTime;
        m_targetPitch += m_inputReader.GetLookVector().y * m_lookSensitivity * Time.unscaledDeltaTime;

        // Clamp rotations
        m_targetYaw = ClampAngle(m_targetYaw, float.MinValue, float.MaxValue);
        m_targetPitch = ClampAngle(m_targetPitch, m_bottomClamp, m_topClamp);

        // Cinemachine will follow this target
        Quaternion targetRotation = Quaternion.Euler(m_targetPitch + CameraAngleOverride, m_targetYaw, 0.0f);
        m_cinemachineTarget.transform.rotation = targetRotation;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        return Mathf.Clamp(angle, min, max);
    }
}