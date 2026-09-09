public class MeteorPolarity : ChromaPolarityBase
{
    // Toàn bộ logic màu (SetColor, UpdateVisual) đã có ở ChromaPolarityBase.
    // Meteor không tự đổi màu — màu được set cố định khi spawn (EnemySpawner/WaveManager gọi SetColor()).
}