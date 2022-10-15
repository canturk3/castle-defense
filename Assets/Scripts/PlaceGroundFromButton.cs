using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaceGroundFromButton : MonoBehaviour// TODO add random grounds 4 blocks long prob.
{
    [SerializeField] private LayerMask Mask;
    [SerializeField] private GameObject objectToBePlaced;
    [SerializeField] private GameObject highlighter;

    private Vector3 movePos;
    private ButtonsManager buttonManager;
    private GridManager gridManager;
    private RaycastHit hit;

    void Start()
    {
        buttonManager = FindObjectOfType<ButtonsManager>();
        gridManager = FindObjectOfType<GridManager>();
        highlightAvailable();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray,out hit, 250.0f,Mask))
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
            && movePos.z >= 0 && movePos.x >= 0 && movePos.z < gridManager.grid.GetLength(0) && movePos.x < gridManager.grid.GetLength(1)
            && placeIsAvailable(Mathf.RoundToInt(transform.position.z), Mathf.RoundToInt(transform.position.x)))
        {
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
            GameObject instObj = Instantiate(objectToBePlaced,transform.position,transform.rotation);

            gridManager.instantiatedGround[(int)transform.position.z, (int)transform.position.x] = instObj;
            gridManager.grid[(int)transform.position.z, (int)transform.position.x] = new Node(objectToBePlaced, (int)transform.position.z, (int)transform.position.x);

            buttonManager.shuffleButtons();
            Destroy(gameObject);
            buttonManager.destroyHighlighters();
        }
    }
    private bool placeIsAvailable(int coordZ, int coordX)
    {
        if (gridManager.grid[coordZ, coordX] == null && hasNeighbour(coordZ, coordX))
        {
            return true;
        }
        return false;
    }
    private bool hasNeighbour(int coordZ, int coordX)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                //Skips diagonal neighbours
                if (Mathf.Abs(i) == Mathf.Abs(j))
                    continue;

                int neighbourGridZ = coordZ + i;
                int neighbourGridX = coordX + j;

                //Controls if the dimensions are inside the grid.
                if (neighbourGridZ >= 0 && neighbourGridZ < gridManager.grid.GetLength(0) && neighbourGridX >= 0 && neighbourGridX < gridManager.grid.GetLength(1))
                {
                    if (gridManager.grid[neighbourGridZ, neighbourGridX] != null)
                    {
                        return true;
                    }
                }
            }
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
                if (placeIsAvailable(i, j))
                {
                    buttonManager.instantiatedHighlighters.Add(Instantiate(highlighter, new Vector3(j, 0, i), Quaternion.identity));
                }
            }
        }
    }
}
