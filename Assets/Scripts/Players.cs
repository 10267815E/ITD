using System;

[Serializable]
public class Player
{
    public string name;
    public int score;

    // Optional: Constructor
    public Player(string name, int score)
    {
        this.name = name;
        this.score = score;
    }

    // Optional: Empty constructor for deserialization
    public Player() { }
}