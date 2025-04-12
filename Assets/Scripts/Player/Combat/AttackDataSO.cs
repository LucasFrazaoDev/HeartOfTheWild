using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Combat/Attack Data")]
public class AttackDataSO : ScriptableObject
{
    public string attackName;
    public AnimationClip attackAnimation;
    public int damage;
}
