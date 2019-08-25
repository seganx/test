using UnityEngine;
using SeganX;

[CreateAssetMenu(menuName = "Game/Tutorial")]
public class TutorialConfig : ScriptableObject
{
    public enum Character : int { Null = 0, Mentor = 1, Mechanic = 2, Driver = 3}

    [PersianPreview]
    public string title = string.Empty;
    [PersianPreview(3)]
    public string description = string.Empty;
    public Character character = Character.Null;
    public TutorialConfig next = null;
}

