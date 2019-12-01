using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FullSerializer;
using Proyecto26;
using System.Globalization;
using UnityEngine.EventSystems;

public class Tutoring : MonoBehaviour
{
    User tutor;
    User student;
    TutoringModel tutoring;

    public Text datutor;
    public Text datutee;
    public Text daclass;
    public Text dadate;
    public Text daattend;

    /* CONFIG */
    private string databaseURL = "https://unity-signup.firebaseio.com/tutoring"; 
    private string AuthKey = "AIzaSyBP8h4aqm8yvWqkoKEhAjCOF9ebnWHk8xU";
    public static fsSerializer serializer = new fsSerializer();


    public ScrollRect scrollView;
    public GameObject scrollContent;
    public GameObject scrollItemPrefab;

    /* INPUT */
    public InputField inputDate;
    public InputField inputTime;
    public Button checkTutoring;
    
    // Start is called before the first frame update
    void Start()
    {
        scrollView.verticalNormalizedPosition = 1;
    }

    public void CheckTutoringButton() {
       CheckTutoring();
    }

    

    IEnumerator RegenerateList()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(1);
        ListTutoring();

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    IEnumerator RegenerateCancelList()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(1);
        CheckTutoring();

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    private void CheckTutoring() {
         RestClient.Get<User>("https://unity-signup.firebaseio.com/users" + "/" + Auth.localId + ".json?auth=" + Auth.idToken).Then(response =>
            {
                student = response; // RESTRICTION NEEDED WHEN NOT LOGEDIN
            }).Catch(error =>
            {
                Debug.Log(error);
            });
            
         RestClient.Get(databaseURL + ".json?auth=" + Auth.idToken).Then(response =>
        {            
            fsData tutoringData = fsJsonParser.Parse(response.Text);
            Dictionary<string, TutoringModel> tutorings = null;
            serializer.TryDeserialize(tutoringData, ref tutorings);

            /* Destroy Precious Content */
            foreach (Transform child in scrollContent.transform) {
                GameObject.Destroy(child.gameObject);
            }

            foreach (var tutoring in tutorings.Values)
            {
                if (tutoring.dadate >= System.DateTime.Now.Ticks && tutoring.datutee.daid == student.daid) {

                    Debug.Log(tutoring.tutoringdb);
                    generateItem(tutoring, "Cancel");
                }
            }
        }).Catch(error =>
        {
            Debug.Log(error);
        });
    }

    private void CancelTutoring(string selectedTutoring) {
        RestClient.Get<TutoringModel>(databaseURL + "/" + selectedTutoring + ".json?auth=" + Auth.idToken).Then(response =>
        {
            tutoring = response;
        }).Catch(error =>
        {
            Debug.Log(error);
        });

        tutoring.datutee = new User("", "", "", "");
        RestClient.Put<TutoringModel>(databaseURL + "/" + tutoring.tutoringdb + ".json?auth=" + Auth.idToken, tutoring);
        StartCoroutine(RegenerateCancelList());

    }

    
    public void TutoringButton() {
        //Debug.Log(EventSystem.current.currentSelectedGameObject.transform.Find("dbinfo").gameObject.GetComponent<Text>().text);
        string buttontype = EventSystem.current.currentSelectedGameObject.transform.Find("description").gameObject.GetComponent<Text>().text;
        string selectedTutoring = EventSystem.current.currentSelectedGameObject.transform.Find("dbinfo").gameObject.GetComponent<Text>().text;
        if (buttontype == "Register") {
            RegisterTutoring(selectedTutoring);
        } else if (buttontype == "Cancel") {
            CancelTutoring(selectedTutoring);
        }
    }
    private void RegisterTutoring(string selectedTutoring) {
        RestClient.Get<User>("https://unity-signup.firebaseio.com/users" + "/" + Auth.localId + ".json?auth=" + Auth.idToken).Then(response =>
        {
            student = response; // RESTRICTION NEEDED WHEN NOT LOGEDIN
        }).Catch(error =>
        {
            Debug.Log(error);
        });


        RestClient.Get<TutoringModel>(databaseURL + "/" + selectedTutoring + ".json?auth=" + Auth.idToken).Then(response =>
        {
            tutoring = response;
        }).Catch(error =>
        {
            Debug.Log(error);
        });

        tutoring.datutee = student;
        RestClient.Put<TutoringModel>(databaseURL + "/" + tutoring.tutoringdb + ".json?auth=" + Auth.idToken, tutoring);
        
        //Regen
        StartCoroutine(RegenerateList());

       




    }
    

    public void ListTutoringButton() {
            ListTutoring();
        }

    private void ListTutoring() {
        RestClient.Get(databaseURL + ".json?auth=" + Auth.idToken).Then(response =>
        {            
            fsData tutoringData = fsJsonParser.Parse(response.Text);
            Dictionary<string, TutoringModel> tutorings = null;
            serializer.TryDeserialize(tutoringData, ref tutorings);

            /* Destroy Precious Content */
            foreach (Transform child in scrollContent.transform) {
                GameObject.Destroy(child.gameObject);
            }

            foreach (var tutoring in tutorings.Values)
            {
                if (tutoring.dadate >= System.DateTime.Now.Ticks && tutoring.datutee.daid == "") {

                    Debug.Log(tutoring.datutee.daid);
                    generateItem(tutoring, "Register");
                }
            }
        }).Catch(error =>
        {
            Debug.Log(error);
        });
    }

    public void postTutoringButton() {
        // Add to restrict unauthroze access.
        RestClient.Get<User>("https://unity-signup.firebaseio.com/users" + "/" + Auth.localId + ".json?auth=" + Auth.idToken).Then(response =>
        {
            tutor = response; // RESTRICTION NEEDED WHEN NOT LOGEDIN
        }).Catch(error =>
        {
            Debug.Log(error);
        });
        string title;
        string [] tutoringDate = inputDate.text.Split('/');
        foreach (string t in tutoringDate) {
            Debug.Log(t);
        }
        string [] tutoringTime = inputTime.text.Split(':');
        foreach (string t in tutoringTime) {
            Debug.Log(t);
        }
        Debug.Log(tutor.daname);
        title = string.Concat(string.Concat(tutoringDate), "-", string.Concat(tutoringTime), "-", tutor.daname);

        long time = new System.DateTime(System.Convert.ToInt32(tutoringDate[0]), System.Convert.ToInt32(tutoringDate[1]), System.Convert.ToInt32(tutoringDate[2]), System.Convert.ToInt32(tutoringTime[0]), System.Convert.ToInt32(tutoringTime[1]), 0).Ticks;
        TutoringModel tutoring = new TutoringModel(tutor, null, time, false, title);
        //Debug.Log(System.DateTime.Now.Ticks >= time);
        PostTutoring(tutoring, title);
    }
    private void PostTutoring(TutoringModel tutoring, string title)
    {
        RestClient.Put<TutoringModel>(databaseURL + "/" + title + ".json?auth=" + Auth.idToken, tutoring);
    }
    

    void generateItem(TutoringModel tutoring, string buttontype) {

        System.DateTime myDate = new System.DateTime(tutoring.dadate); // Date ticks
        string cleanedDate = myDate.ToString("F", CultureInfo.CreateSpecificCulture("en-US"));

        GameObject scrollItemObj = Instantiate(scrollItemPrefab);
        scrollItemObj.transform.SetParent(scrollContent.transform, false);
        scrollItemObj.transform.Find("datetime").gameObject.GetComponent<Text>().text = cleanedDate;
        scrollItemObj.transform.Find("tutor").gameObject.GetComponent<Text>().text = tutoring.datutor.daname;
        scrollItemObj.transform.Find("register").transform.Find("description").gameObject.GetComponent<Text>().text = buttontype;
        scrollItemObj.transform.Find("register").transform.Find("dbinfo").gameObject.GetComponent<Text>().text = tutoring.tutoringdb;
        scrollItemObj.transform.Find("register").gameObject.GetComponent<Button>().onClick.AddListener(TutoringButton);
    }

}
