
using Unity.VisualScripting;
using UnityEngine;

public static class Util
{
    public static Color SolvedCharColor = new Color(0.4f, 0.4f, 0.4f, 1f);
    public static Color UncertainCharColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    
    public static string FormatSolvedChar(char solvedCharacter) => $"<color=#{SolvedCharColor.ToHexString()}>{solvedCharacter}</color>";
    public static string FormatUncertainChar(char uncertainCharacter) => $"<color=#{UncertainCharColor.ToHexString()}>{uncertainCharacter}</color>";
    public static string FormatStartChar(char starterCharacter) => $"<b>{starterCharacter}</b>";
}
