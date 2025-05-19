using UnityEngine;

public class SpiderAttackHitbox : MonoBehaviour
{
    [SerializeField] private float _damage = 10f;
    private Collider _attackCollider;

    private void Awake()
    {
        _attackCollider = GetComponent<Collider>();
        _attackCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider target)
    {
        // Ignora colisões com o próprio inimigo
        if (target.transform.root == transform.root)
            return;

        var damageable = target.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Debug.Log("Pegou no player");
            damageable.TakeDamage(_damage);
        }
    }
}