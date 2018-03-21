using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct HexCoordinates { 

    [SerializeField]
    public int x { get; private set; }

    [SerializeField]
    public int y { get; private set; }

    [SerializeField]
    public int z { get; private set; }
    public HexCoordinates(int _x, int _y, int _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    public static HexCoordinates formOffsetCoordinates(int x, int z)
    {
        int offsetX = (x - z / 2);
        int offsetY = -offsetX - z;

        return new HexCoordinates(offsetX, offsetY, z);
    }
}
