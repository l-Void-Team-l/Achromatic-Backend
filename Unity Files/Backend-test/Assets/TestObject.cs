using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TestObject", order = 1)]
public class TestObject : ScriptableObject
{
    public string id;
    public int score;
}