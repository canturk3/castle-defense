using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBlueprint : MonoBehaviour
{
    public GameObject tempObject;
    private static GameObject previousBP;//static since it is shared between all buttons.
    private ButtonsManager buttonManager;
    private void Start()
    {
        buttonManager = FindObjectOfType<ButtonsManager>();
    }

    public void creatObject()
    {
        if (previousBP != null)
        {
            Destroy(previousBP);
            buttonManager.destroyHighlighters();
        }
        previousBP = Instantiate(tempObject);
    }
}
