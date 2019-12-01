using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollScript : MonoBehaviour
{
    public ScrollRect scrollView;
    public GameObject scrollContent;
    public GameObject scrollItemPrefab;
    // Start is called before the first frame update
    void Start()
    {
        for(int a = 1; a <= 20; a++) {
            generateItem(a);
        }
        scrollView.verticalNormalizedPosition = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void generateItem(int itemNumber) {
        GameObject scrollItemObj = Instantiate(scrollItemPrefab);
        scrollItemObj.transform.SetParent(scrollContent.transform, false);
        scrollItemObj.transform.Find("num").gameObject.GetComponent<Text>().text = itemNumber.ToString();
    }



}
