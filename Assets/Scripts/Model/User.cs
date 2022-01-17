using UniRx;

/// <summary>
/// ���[�U�[��Model
/// </summary>
[System.Serializable]
public class User {
    public ReactiveProperty<int> Food;
    public ReactiveProperty<int> Coin;
    public ReactiveProperty<int> Level;   // ���݂̃X�e�[�W��

    /// <summary>
    /// ���[�U�[�̍쐬
    /// </summary>
    /// <param name="food"></param>
    /// <returns></returns>
    public static User CreateUser(int food, int coin = 0, int level  =1) {
        User user = new User {
            Food = new ReactiveProperty<int>(food),
            Coin = new ReactiveProperty<int>(coin),
            Level = new ReactiveProperty<int>(level)
        };
        return user;
    }
}
