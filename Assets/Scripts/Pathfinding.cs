using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private GridManager gridManager;
    private List<Node> path;

    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
    }
    public List<Node> getPath()
    {
        return path;
    }

    public List<Node> FindPath(Vector2 startPos, Vector2 targetPos)
    {
        Node startNode = gridManager.grid[(int)startPos.y,(int)startPos.x];
        Node targetNode = gridManager.grid[(int)targetPos.y, (int)targetPos.x];

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].Hcost < currentNode.Hcost)
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                CreatePath(startNode, targetNode);
                return path;
            }

            foreach (Node neighbour in gridManager.getNeighbours(currentNode))
            {
                if (!neighbour.Walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }
                int newMovementCostToNeighbour = currentNode.Gcost + GetDistance(currentNode, neighbour);

                if (newMovementCostToNeighbour < neighbour.Gcost || !openSet.Contains(neighbour))
                {
                    neighbour.Gcost = newMovementCostToNeighbour;
                    neighbour.Hcost = GetDistance(neighbour, targetNode);
                    neighbour.Parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
        return null;
    }

    void CreatePath(Node startNode, Node endNode)
    {
        path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstZ = Mathf.Abs(nodeA.CoordZ - nodeB.CoordZ);
        int dstX = Mathf.Abs(nodeA.CoordX - nodeB.CoordX);

        if (dstZ > dstX)
        {
            return 14 * dstX + 10 * (dstZ - dstX);
        }
        return 14 * dstZ + 10 * (dstX - dstZ);
    }
}

