using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeleteObjectFromButton : MonoBehaviour
{
    [SerializeField] private LayerMask Mask;
    [SerializeField] private GameObject highlighter;

    private RaycastHit hit;
    private Vector3 movePos;
    private ButtonsManager buttonManager;
    private GridManager gridManager;

    void Start()
    {
        buttonManager = FindObjectOfType<ButtonsManager>();
        gridManager = FindObjectOfType<GridManager>();
        highlightAvailable();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 250.0f, Mask) && hit.point.x >= 0 && hit.point.z >= 0)
        {
            transform.position = hit.point;
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 250.0f, Mask))
        {
            movePos = new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y) + 1, Mathf.Round(hit.point.z));
            transform.position = Vector3.Lerp(transform.position, movePos, 0.5f);
        }
        if (Input.GetKey("escape"))
        {
            Destroy(gameObject);
            buttonManager.destroyHighlighters();
        }

        else if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()
            && movePos.z >= 0 && movePos.x >= 0 && movePos.z < gridManager.grid.GetLength(0) && movePos.x < gridManager.grid.GetLength(1)
            && canDelete(Mathf.RoundToInt(transform.position.z), Mathf.RoundToInt(transform.position.x)))
        {

            Destroy(gridManager.instantiatedAboveGround[Mathf.RoundToInt(transform.position.z), Mathf.RoundToInt(transform.position.x)]);
            clearNodes(Mathf.RoundToInt(transform.position.z), Mathf.RoundToInt(transform.position.x));

            buttonManager.shuffleButtons();
            Destroy(gameObject);
            buttonManager.destroyHighlighters();
        }
    }
    private bool canDelete(int coordZ,int coordX)
    {
        if (gridManager.grid[coordZ,coordX] != null
            && gridManager.instantiatedAboveGround[coordZ, coordX] != null
            && !gridManager.instantiatedAboveGround[coordZ, coordX].name.Equals("Castle(Clone)"))
        {
            return true;
        }
        return false;
    }

    private void clearNodes(int z,int x)//may be unnecessarily complex that can delete z shaped figures.
    {
        Node firstNode = gridManager.grid[z, x];

        List<Node> nodesToClear = new List<Node> { firstNode };
        List<Node> neighboursFound = new List<Node> { firstNode };

        List<Node> currentNeighbours;
        do
        {
            currentNeighbours = gridManager.getNeighbours(neighboursFound[neighboursFound.Count - 1]);
            for (int i = 0; i < currentNeighbours.Count; i++)
            {
                if (currentNeighbours[i] == firstNode && !nodesToClear.Contains(currentNeighbours[i]))
                {
                    neighboursFound.Add(currentNeighbours[i]);
                    nodesToClear.Add(currentNeighbours[i]);
                }
            }
           
            neighboursFound.RemoveAt(0);
        } while (neighboursFound.Count != 0);

        foreach (Node node in nodesToClear)
        {
            gridManager.instantiatedAboveGround[node.CoordZ, node.CoordX] = null;
            gridManager.grid[node.CoordZ, node.CoordX].Walkable = true;
        }
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
                    buttonManager.instantiatedHighlighters.Add(Instantiate(highlighter, new Vector3(j, 1, i), Quaternion.identity));
                }
            }
        }
    }
}
