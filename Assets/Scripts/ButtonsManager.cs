using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsManager : MonoBehaviour// TODO butonlara basınca eğer altında bir şey varsa ona da basılıyor
{
    public List<GameObject> buttons = new List<GameObject>();
    public List<GameObject> instantiatedHighlighters = new List<GameObject>();

    [SerializeField] private int waitSeconds = 1;


    private void Awake()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            buttons.Add(gameObject.transform.GetChild(i).gameObject);
        }
        setAllInactive();
    }

    private void Start()
    {
        buttons[0].transform.position = new Vector3(300, 40);
        buttons[0].SetActive(true);
    }

    public void setAllInactive()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetActive(false);
        }
    }
    public void shuffleButtons()
    {
        setAllInactive();
        StartCoroutine(shuffler());
    }

    private IEnumerator shuffler()
    {
        yield return new WaitForSecondsRealtime(waitSeconds);
        shuffle();
    }

    private void shuffle()// TODO add probabilites
    {
        System.Random rnd = new System.Random();
        List<GameObject> chosenBts = new List<GameObject>();

        setAllInactive();

        for (int i = 0; i < 3; i++)
        {
            int chooseButton = rnd.Next(buttons.Count);

            buttons[chooseButton].transform.position = new Vector3(200 + (i * 100), 40);
            buttons[chooseButton].SetActive(true);


            chosenBts.Add(buttons[chooseButton]);
            buttons.RemoveAt(chooseButton);
        }

        while (chosenBts.Count != 0)
        {
            buttons.Add(chosenBts[0]);
            chosenBts.RemoveAt(0);
        }
    }
    public void destroyHighlighters()
    {
        while (instantiatedHighlighters.Count > 0)
        {
            Destroy(instantiatedHighlighters[instantiatedHighlighters.Count - 1]);
            instantiatedHighlighters.RemoveAt(instantiatedHighlighters.Count - 1);
        }
    }
}
