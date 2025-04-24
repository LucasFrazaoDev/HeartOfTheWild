using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackData", menuName = "Combat/Attack Data")]
public class AttackDataSO : ScriptableObject
{
    public WeaponType weaponType;
    public AttackCombo[] comboSequence;

    public AttackCombo GetAttackData(int comboIndex)
    {
        if (comboIndex < 0 || comboIndex >= comboSequence.Length)
            return null;

        return comboSequence[comboIndex];
    }
}