using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaceObjectFromButton : MonoBehaviour
{
    [SerializeField] private LayerMask Mask;
    [SerializeField] private GameObject objectToBePlaced;
    [SerializeField] private int objZdimension,objXdimension;//TODO L and z shaped must be added
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
            transform.position = hit.point;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 250.0f, Mask))
        {
            movePos = new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y) +0.5f, Mathf.Round(hit.point.z));
            transform.position = Vector3.Lerp(transform.position, movePos, 0.5f);
        }

        if (Input.GetKey("escape"))
        {
            Destroy(gameObject);
            buttonManager.destroyHighlighters();
        }
        else if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() && movePos.z >= 0 && movePos.x >= 0 && movePos.z < gridManager.grid.GetLength(0) && movePos.x < gridManager.grid.GetLength(1) &&
            placeIsAvailable(Mathf.RoundToInt(transform.position.z), Mathf.RoundToInt(transform.position.x)))
        {

            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y) + 0.5f, Mathf.Round(transform.position.z));
            instantiate();

            if (gameObject.name.Equals("CastleBP(Clone)"))
            {
                gridManager.castleFront = new Vector2 ( Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z) - 1);
                buttonManager.buttons[0].SetActive(false);
                buttonManager.buttons.RemoveAt(0);
            }
            buttonManager.shuffleButtons();
            Destroy(gameObject);
            buttonManager.destroyHighlighters();

        }
    }
    private bool placeIsAvailable(int coordZ,int coordX)
    {
        bool canPlace = true;
        for (int i = 0; i < objZdimension;i++)
        {
            for (int j = 0; j < objXdimension; j++)
            {
                Node gridNode = gridManager.grid[coordZ + i, coordX + j];
                if (gridNode == null ||
                    gridManager.instantiatedAboveGround[coordZ + i, coordX + j] != null)
                {
                    canPlace = false;
                    break;
                }

                if (!gameObject.name.Equals("CastleBP(Clone)") &&
                    ((gridNode.CoordX == gridManager.castleFront.x &&
                    gridNode.CoordZ == gridManager.castleFront.y) ||
                    (gridNode.CoordX == gridManager.castleFront.x + 1 && gridNode.CoordZ == gridManager.castleFront.y) || 
                     willCutOffNeighbours(gridNode.CoordZ, gridNode.CoordX)))
                {
                    canPlace = false;
                    break;
                }
                else if (gameObject.name.Equals("CastleBP(Clone)") && gridManager.getNeighbours(gridNode).Count != 4)
                {
                    canPlace = false;
                    break;
                }
            }
            if (!canPlace)
            {
                break;
            }
        }

        return canPlace;
    }
    private void instantiate()
    {
        GameObject instObj = Instantiate(objectToBePlaced, transform.position, transform.rotation);
        for (int i = 0; i < objZdimension; i++)
        {
            for (int j = 0; j < objXdimension; j++)
            {
                gridManager.instantiatedAboveGround[(int)transform.position.z + i, (int)transform.position.x + j] = instObj;
                gridManager.grid[(int)transform.position.z + i, (int)transform.position.x + j].Walkable = false;
            }
        }
    }
    private bool willCutOffNeighbours(int coordZ, int coordX)
    {
        List<Node> neighbours = gridManager.getNeighbours(gridManager.grid[coordZ, coordX]);

        Node tempHoldDeleted = new Node(gridManager.grid[coordZ, coordX].Element, coordZ, coordX);

        gridManager.grid[coordZ, coordX] = null;
        bool willCut = false;

        foreach (Node neighbour in neighbours)
        {
            if (neighbour.Walkable && pathfinding.FindPath(new Vector2(neighbour.CoordX, neighbour.CoordZ), gridManager.castleFront) == null)
            {
                willCut = true;
                break;
            }
        }
        gridManager.grid[coordZ, coordX] = tempHoldDeleted;
        return willCut;
    }

    private void highlightAvailable()
    {
        Node[,] grid = gridManager.grid;

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] != null && placeIsAvailable(i, j))
                {
                    for (int k = 0; k < objZdimension; k++)
                    {
                        for (int l = 0; l < objXdimension; l++)
                        {
                            buttonManager.instantiatedHighlighters.Add(Instantiate(highlighter, new Vector3(j + l, 0, i + k), Quaternion.identity));
                        }
                    }
                }
            }
        }
    }
}
