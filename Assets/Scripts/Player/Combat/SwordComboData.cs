using UnityEngine;

[CreateAssetMenu(fileName = "SwordComboData", menuName = "Combat/Sword Combo Data")]
public class SwordComboData : ScriptableObject
{
    public AttackCombo[] comboSequence;

    public AttackCombo GetAttackData(int comboIndex)
    {
        if (comboIndex < 0 || comboIndex >= comboSequence.Length)
            return null;

        return comboSequence[comboIndex];
    }
}