using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;

public enum Align { Center, Left, Right, Up, Down }
public enum TutorialDir { Left, Right, Up, Down }

[CreateAssetMenu(menuName = "Tutorial Config")]
public class TutorialConfig : ScriptableObject
{
    #region fields
    public int tutorialIndex;
    public Align align;
    public string dialogueString;
    public Vector2 dialoguePosition;
    public TutorialPointer tutorialPointer;
    public TutorialConfig nextTutorialConfig;
    public Rect focusRect;
    #endregion
}

[System.Serializable]
public class TutorialPointer
{
    public Vector2 position;
    public TutorialDir dir;
}