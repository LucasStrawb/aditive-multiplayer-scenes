using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SetData", menuName = "ScriptableObjects/SetData", order = 1)]
public class SetData : ScriptableObject
{
    public GameObject[] GameObjects => _gameObjects;
    [SerializeField] private GameObject[] _gameObjects;
}
