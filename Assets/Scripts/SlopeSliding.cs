using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SlopeSliding : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 hitNormal; // Normal do terreno (direção da inclinação)

    [Header("Configurações de Deslize")]
    [Tooltip("Velocidade do deslize em terrenos íngremes.")]
    public float slideSpeed = 8f;
    [Tooltip("Inclinação mínima para iniciar o deslize (0.1 = leve, 0.9 = quase vertical).")]
    [Range(0.1f, 0.9f)] public float slopeThreshold = 0.5f;
    [Tooltip("Distância do raycast para detectar o chão.")]
    public float raycastDistance = 1.5f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (IsOnSteepSlope() && controller.isGrounded)
        {
            SlideDown();
        }
    }

    bool IsOnSteepSlope()
    {
        // Raycast para baixo para detectar o terreno
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, raycastDistance))
        {
            hitNormal = hit.normal;
            float slopeAngle = Vector3.Angle(hitNormal, Vector3.up);
            float slopeY = Mathf.Abs(hitNormal.y); // Quanto menor, mais íngreme

            // Verifica se o ângulo é maior que o slopeLimit do Controller E se a inclinação é acentuada
            return (slopeAngle > controller.slopeLimit) && (slopeY < slopeThreshold);
        }
        return false;
    }

    void SlideDown()
    {
        // Calcula a direção do deslize (inverso da normal do terreno, com componente horizontal)
        Vector3 slideDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z).normalized;
        controller.Move(slideDirection * slideSpeed * Time.deltaTime);
    }

    // Debug: Mostra a normal do terreno no Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + hitNormal * 2f);
    }
}