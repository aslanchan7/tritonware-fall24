using UnityEngine;

public interface IDamageable
{
    public Team Team { get; }
    public Vector2Int Pos { get; set; }
    public void Damage(int value);
    public void Heal(int value);
}
