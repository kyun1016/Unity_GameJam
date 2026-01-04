public class Enum
{
    public enum Languege
    {
        Korean,
        English,
        Japanese,
        Chinese
    }
    public enum ItemType
    {
        None,
        Health,
        Mana,
        Stamina,
        Strength,
        Agility,
        Intelligence
    }
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Victory
    }
    public enum EnemyType
    {
        Goblin,
        Orc,
        Troll,
        Dragon,
        Undead
    }
    public enum SFX
    {
        Attack,
        Jump,
        Damage,
        Death,
        Heal,
        UI_Click,
        LevelUp
    }
    public enum CursorType
    {
        Default,
        Hover,
        Click,
        Attack,
        Disabled
    }
}