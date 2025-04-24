[System.Serializable]
public class AttackCombo
{
    public string animationTrigger;
    public float damage;
    public float attackDuration;
    public float comboWindow; // Tempo para próxima entrada do combo
    public bool isFinalAttack; // Se é o último ataque do combo
}