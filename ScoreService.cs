using System;
using System.Diagnostics;


public interface IServiceScore
{
    void Add(int pPoints);
    int Get();
}

class ScoreService : IServiceScore
{
    private int Value;

    public ScoreService()
    {
        Value = 0;
        // ServiceLocator.RegisterService<IServiceScore>(this);
    }
    public void Add(int pPoints)
    {
        Value += pPoints;
    }

    public int Get()
    {
        return Value;
    }
    public void Display()
    {
        Debug.WriteLine(Value);
    }

}