using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    enum FruitType
    {
        Cherry = 100,
        Strawberry = 300,
        Orange = 500,
        Apple = 700,
        Pear = 1000,
        Banana = 2000,
    }

    [SerializeField]
    private FruitType type;

    public int GetPoints()
    {
        return (int)type;
    }

}
