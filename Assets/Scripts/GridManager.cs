using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int columnLength = 12, rowLength = 12;
    [SerializeField] private GameObject gridElement;

    //i = z && j = x
    public Node[,] grid = new Node[50,50];
    public GameObject[,] instantiatedGround = new GameObject[50,50];
    public GameObject[,] instantiatedAboveGround = new GameObject[50, 50];

    public Vector2 castleFront;//in front of right door

    void Awake()
    {
        int startPointZ = 25;
        int startPointX = 25;
        
        for (int i = 0; i < rowLength; i++)//initialize the area
        {
            for (int j = 0; j < columnLength; j++)
            {
                Node newNode = new Node(gridElement, i + startPointZ, j + startPointX);
                grid[i + startPointZ, j + startPointX] = newNode;
                GameObject instVar = Instantiate(grid[i + startPointZ, j + startPointX].Element, new Vector3(j + startPointX, 0, i + startPointZ), Quaternion.identity);
                instantiatedGround[i + startPointZ, j + startPointX] = instVar;
            }
        }
    }

    /*public void assignTheEdges()//units will spawn and delete will happen
    {
        bool firstEncRow = false, lastEncRow = false;
        bool firstTime = true;
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i,j] != null)
                {
                    if (firstTime)
                    {
                        firstEncRow = true;
                        firstTime = false;
                    }
                    if (firstEncRow)
                    {
                        grid[i, j].IsOnFringes = true;
                    }
                }
                
            }
            if (firstEncRow)
            {
                firstEncRow = false;
            }
        }
    }*/

    public List<Node> getNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                //Skips diagonal neighbours
                if (Mathf.Abs(i) == Mathf.Abs(j))
                    continue;
                
                int neighbourGridZ = node.CoordZ + i;
                int neighbourGridX = node.CoordX + j;

                //Controls if the dimensions are inside the grid.
                if (neighbourGridZ >= 0 && neighbourGridZ < grid.GetLength(0) && neighbourGridX >= 0 && neighbourGridX < grid.GetLength(1))
                {
                    if (grid[neighbourGridZ, neighbourGridX] != null)
                    {
                        neighbours.Add(grid[neighbourGridZ, neighbourGridX]);
                    }
                }
            }
        }
        return neighbours;
    }

    public int getDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.CoordZ - nodeB.CoordZ);
        int dstY = Mathf.Abs(nodeA.CoordX - nodeB.CoordX);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
