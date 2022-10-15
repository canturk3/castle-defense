using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private GameObject element;
    private GameObject aboveGroundObject;
    private Node parent;
    private int coordX, coordZ,gCost,hCost;
    private bool walkable;
    private bool isOnFringes;// TODO may not be necessary check later

    public Node(GameObject element,int coordZ,int coordX)
    {
        this.coordX = coordX;
        this.coordZ = coordZ;
        this.element = element;
        walkable = true;
        isOnFringes = false;
    }
    public GameObject AboveGroundObject {  get{ return aboveGroundObject; } set {aboveGroundObject = value; } }
    public GameObject Element { get { return element; } set { element = value; } }
    public Node Parent { get {return parent; } set {parent = value; } }
    public bool Walkable { get {return walkable; } set {walkable = value; } }
    public bool IsOnFringes { get { return isOnFringes; } set { isOnFringes = value; } }
    public int CoordX { get {return coordX; } set {coordX = value; } }
    public int CoordZ { get { return coordZ; } set { coordZ = value; } }
    public int Gcost { get {return gCost; } set {gCost = value; } }
    public int Hcost { get { return hCost; } set { hCost = value; } }

    public int fCost { get { return gCost + hCost; } }
}
