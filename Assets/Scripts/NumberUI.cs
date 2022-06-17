using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberUI : MonoBehaviour
{
    public GameObject n0;
    public GameObject n1;
    public GameObject n2;
    public GameObject n3;
    public GameObject n4;

    public GameObject n5;

    public int value;
    // Start is called before the first frame update
    void Start()
    {
        value = 1;
        SetUI();
    }

    public void SetUI()
    {
        n0.SetActive(false);
        n1.SetActive(false);
        n2.SetActive(false);
        n3.SetActive(false);
        n4.SetActive(false);
        n5.SetActive(false);
        switch (value)
        {
                case 0: n0.SetActive(true);
                    break;
                case 1: n1.SetActive(true);
                    break;
                case 2: n2.SetActive(true);
                    break;
                case 3: n3.SetActive(true);
                    break;
                case 4: n4.SetActive(true);
                    break;
                case 5: n5.SetActive(true);
                    break;
               
        }
    }
  
}
