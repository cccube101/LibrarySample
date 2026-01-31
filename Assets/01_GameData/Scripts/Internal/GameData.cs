using R3;

public class GameData
{
    // ---------------------------- Field
    private readonly ReactiveProperty<float> _score = new(2000);
    public ReadOnlyReactiveProperty<float> Score => _score;



    // ---------------------------- PublicMethod
    public void AddScore(float add)
    {
        _score.Value += add;
    }

    public void SubScore(float sub)
    {
        _score.Value -= sub;
    }


    // ---------------------------- PrivateMethod
}