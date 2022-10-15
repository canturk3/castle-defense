using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class DeleteGroundFromButton : MonoBehaviour // TODO prevent cutting of sections
{
    [SerializeField] private LayerMask Mask;
    [SerializeField] private GameObject highlighter;

    private RaycastHit hit;
    private Vector3 movePos;
    private ButtonsManager buttonManager;
    private GridManager gridManager;
    private Pathfinding pathfinding;

    void Start()
    {
        buttonManager = FindObjectOfType<ButtonsManager>();
        gridManager = FindObjectOfType<GridManager>();
        pathfinding = FindObjectOfType<Pathfinding>();
        highlightAvailable();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 250.0f, Mask))
        {
            transform.position = hit.point;
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 250.0f, Mask))
        {
            movePos = new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z));
            transform.position = Vector3.Lerp(transform.position, movePos, 0.5f);
        }

        if (Input.GetKey("escape"))
        {
            Destroy(gameObject);
            buttonManager.destroyHighlighters();
        }

        else if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()
            && movePos.z >= 0 && movePos.x >= 0 && movePos.z < gridManager.grid.GetLength(0) && movePos.x < gridManager.grid.GetLength(1) &&
            canDelete(Mathf.RoundToInt(transform.position.z), Mathf.RoundToInt(transform.position.x)))
        {
            Destroy(gridManager.instantiatedGround[Mathf.RoundToInt(transform.position.z), Mathf.RoundToInt(transform.position.x)]);

            gridManager.grid[Mathf.RoundToInt(transform.position.z), Mathf.RoundToInt(transform.position.x)] = null;
            buttonManager.shuffleButtons();
            Destroy(gameObject);
            buttonManager.destroyHighlighters();
        }
    }
    private bool canDelete(int coordZ, int coordX)
    {
        if (gridManager.grid[coordZ, coordX] != null &&
            gridManager.instantiatedAboveGround[coordZ, coordX] == null &&
            !((coordX == gridManager.castleFront.x && coordZ == gridManager.castleFront.y) ||
            (coordX == gridManager.castleFront.x + 1 && coordZ == gridManager.castleFront.y) ||
            willCutOffNeighbours(coordZ, coordX)))
        {
            return true;
        }
        return false;
    }
    
    private bool willCutOffNeighbours(int coordZ,int coordX)
    {
        List<Node> neighbours = gridManager.getNeighbours(gridManager.grid[coordZ,coordX]);

        Node tempHoldDeleted = new Node(gridManager.grid[coordZ, coordX].Element,coordZ,coordX);

        gridManager.grid[coordZ, coordX] = null;

        if (!createsObjectIsland(neighbours))
        {
            bool willCut = false;

            foreach (Node neighbour in neighbours)//cuts neighbours off
            {
                List<Node> path = pathfinding.FindPath(new Vector2(neighbour.CoordX, neighbour.CoordZ), gridManager.castleFront);

                if (neighbour.Walkable && path == null)
                {
                    willCut = true;
                    break;
                }
            }
            gridManager.grid[coordZ, coordX] = tempHoldDeleted;
            return willCut;
        }
        else
        {
            gridManager.grid[coordZ, coordX] = tempHoldDeleted;
            return true;
        }
    }

    private bool createsObjectIsland(List<Node> neighbours)//initial neighbours of a removed node must be used in willCutoffNeighbours
    {
        Node currentNode = null;
        bool possibleIsland = false;
        for (int i = 0; i < neighbours.Count; i++)
        {
            if (!neighbours[i].Walkable)
            {
                possibleIsland = true;
                currentNode = neighbours[i];
            }
        }

        if (possibleIsland)
        {
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(currentNode);
            while (openSet.Count > 0)
            {
                currentNode = openSet[0];


                openSet.Remove(currentNode);
                closedSet.Add(currentNode);


                currentNode.Walkable = true;
                if (pathfinding.FindPath(new Vector2(currentNode.CoordX, currentNode.CoordZ), gridManager.castleFront) != null)
                {
                    currentNode.Walkable = false;
                    return false;
                }
                currentNode.Walkable = false;

                foreach (Node neighbour in gridManager.getNeighbours(currentNode))
                {
                    if (neighbour.Walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);                     
                    }
                }
            }
            return true;
        }
        return false;
    }
    private void highlightAvailable()
    {
        Node[,] grid = gridManager.grid;

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (canDelete(i, j))
                {
                    buttonManager.instantiatedHighlighters.Add(Instantiate(highlighter, new Vector3(j, 0, i), Quaternion.identity));
                }
            }
        }
    }
}
