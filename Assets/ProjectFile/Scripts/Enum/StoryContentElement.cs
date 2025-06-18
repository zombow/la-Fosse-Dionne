using UnityEngine;

public enum StoryElementType
{
    Text,
    Image
}

[System.Serializable]
public class StoryContentElement
{
    public StoryElementType type;
    [TextArea] public string text;
    public Sprite image;
}