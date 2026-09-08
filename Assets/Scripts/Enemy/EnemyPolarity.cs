using UnityEngine;

public class EnemyPolarity : ChromaPolarityBase
{
    // Toàn bộ hành vi màu sắc (SetColor, SwitchColor, UpdateVisual) đã có ở ChromaPolarityBase.
    // SubBossEnemy có thể gọi SwitchColor() hoặc SetColor() theo timer riêng của nó (armorColorTimer)
    // mà không cần định nghĩa lại logic màu ở đây.
}