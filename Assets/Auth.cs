using System.Collections;
using System.Collections.Generic;
using FullSerializer;
using Proyecto26;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Auth : MonoBehaviour
{
    public InputField daid;
    public InputField daname;
    public InputField dapassword;
    public InputField daclass;
    public InputField daemail;
    
    User user;

    /* CONFIGURATION */
    private string databaseURL = "https://unity-signup.firebaseio.com/users"; 
    private string AuthKey = "AIzaSyBP8h4aqm8yvWqkoKEhAjCOF9ebnWHk8xU";
    public static fsSerializer serializer = new fsSerializer();
    //suppose IDTOKEN IS PRIVATE> FOR TEST PURPOSE ITS PUBLIC AND ALSO IT IS NOT STATIC
    public static string idToken;    
    public static string localId;
    private string getLocalId;
    

    /* DISPLAY MODULE */
    public static string displayId;
    public static string displayName; 
    public static string displayClass;

    private void Start()
    {
        displayName = "Test";

    }

    public void OnSubmit()
    {
        PostToDatabase();
    }
    
    public void OnGetScore()
    {
        GetLocalId();
    }

    private void PostToDatabase(bool emptyScore = false)
    {
        User user = new User(displayId, displayName, displayClass, localId);
        RestClient.Put(databaseURL + "/" + localId + ".json?auth=" + idToken, user);
    }

    private void RetrieveFromDatabase()
    {
        RestClient.Get<User>(databaseURL + "/" + getLocalId + ".json?auth=" + idToken).Then(response =>
            {
                user = response;
            });
    }

    public void SignUpUserButton()
    {
        SignUpUser(daid.text, dapassword.text, daname.text, daemail.text, daclass.text);
    }
    
    public void SignInUserButton()
    {   
        SignInUser(daemail.text, dapassword.text);
    }
    
    private void SignUpUser(string daid, string dapassword, string daname, string daemail, string daclass)
    {
        string userData = "{\"email\":\"" + daemail + "\",\"password\":\"" + dapassword + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=" + AuthKey, userData).Then(
            response =>
            {
                idToken = response.idToken;
                localId = response.localId;
                displayName = daname;
                displayId = daid;
                displayClass = daclass;
                Debug.Log(displayClass);
                PostToDatabase(true);
            }).Catch(error =>
        {
            Debug.Log(error);
        });
    }
    
    private void SignInUser(string daemail, string dapassword)
    {
        string userData = "{\"email\":\"" + daemail + "\",\"password\":\"" + dapassword + "\",\"returnSecureToken\":true}";
        Debug.Log(userData);
        RestClient.Post<SignResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=" + AuthKey, userData).Then(
            response =>
            {
                idToken = response.idToken;
                localId = response.localId;
                GetUsername();
            }).Catch(error =>
        {
            Debug.Log(error);
        });
    }

    private void GetUsername()
    {
        RestClient.Get<User>(databaseURL + "/" + localId + ".json?auth=" + idToken).Then(response =>
        {
            displayName = response.daname;
            displayId = response.daid;
        });
    }
    
    private void GetLocalId(){
        RestClient.Get(databaseURL + ".json?auth=" + idToken).Then(response =>
        {            
            fsData userData = fsJsonParser.Parse(response.Text);
            Dictionary<string, User> users = null;
            serializer.TryDeserialize(userData, ref users);

            foreach (var user in users.Values)
            {
                if (user.daid == daid.text)
                {
                    getLocalId = user.localId;
                    RetrieveFromDatabase();
                    break;
                }
            }
        }).Catch(error =>
        {
            Debug.Log(error);
        });
    }

    public void ListUserButton() {
        ListUser();
    }

    private void ListUser() {
        RestClient.Get(databaseURL + ".json?auth=" + idToken).Then(response =>
        {            
            fsData userData = fsJsonParser.Parse(response.Text);
            Dictionary<string, User> users = null;
            serializer.TryDeserialize(userData, ref users);

            foreach (var user in users.Values)
            {
                Debug.Log(user.daname);
            }
        }).Catch(error =>
        {
            Debug.Log(error);
        });
    }
}