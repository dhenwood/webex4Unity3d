using System.Net;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;



public class webexTeams : MonoBehaviour
{

    public string bearerToken;
    public string roomId;
    public int maxMessage;

    private Text myText;
    private GameObject Item1Text1;
    public Image Avatar1;
    public Image Avatar2;
    public Image Avatar3;
    public Image Avatar4;
    public Image Avatar5;

    void Start()
    {
        ListMessages();

    }

    void Update()
    {
        
    }

    void ListMessages()
    {
        Debug.Log("grabbing Messages...");

        List<Dictionary<string, string>> messageArray = new List<Dictionary<string, string>>();

        var webRequest = System.Net.WebRequest.Create("https://api.ciscospark.com/v1/messages?roomId=" + roomId +"&max=" + maxMessage);
        webRequest.Method = "GET";
        webRequest.Timeout = 10000;
        webRequest.ContentType = "application/json";
        webRequest.Headers.Add("Authorization", "Bearer " + bearerToken);

        HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        Debug.Log("Resonse: " + jsonResponse);

        var jsonParsed = JSON.Parse(jsonResponse);
        var jsonEachMessage = jsonParsed["items"];
        int messagesReturned = (jsonEachMessage.Count);
         
        for (int i = 0; i < messagesReturned; i++)
        {
            string textMessage = jsonEachMessage[i]["text"];
            string emailMessage = jsonEachMessage[i]["personEmail"];

            Dictionary<string, string> myDictionary = new Dictionary<string, string>() { { "text", textMessage }, { "personEmail", emailMessage } };
            messageArray.Add(myDictionary);
        }

        updateText(messageArray[0]["text"], "Item1Text1");
        updateText(messageArray[1]["text"], "Item2Text1");
        updateText(messageArray[2]["text"], "Item3Text1");
        updateText(messageArray[3]["text"], "Item4Text1");
        updateText(messageArray[4]["text"], "Item5Text1");

        getPersonDetails("0",messageArray[0]["personEmail"]);
        getPersonDetails("1",messageArray[1]["personEmail"]);
        getPersonDetails("2",messageArray[2]["personEmail"]);
        getPersonDetails("3",messageArray[3]["personEmail"]);
        getPersonDetails("4",messageArray[4]["personEmail"]);
    }

    void getPersonDetails(string itemNumber, string personEmail){
        Debug.Log("grabbing Person Details...");

        string myPeopleUrl = "https://api.ciscospark.com/v1/people?email=" + personEmail;
        var webRequest = System.Net.WebRequest.Create(myPeopleUrl);
        webRequest.Method = "GET";
        webRequest.Timeout = 10000;
        webRequest.ContentType = "application/json";
        webRequest.Headers.Add("Authorization", "Bearer " + bearerToken);

        HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();

        var jsonParsed = JSON.Parse(jsonResponse);
        var jsonEachMessage = jsonParsed["items"];
        int messagesReturned = (jsonEachMessage.Count);
         
         
        string displayName = jsonEachMessage[0]["displayName"];
        string avatarUrl = jsonEachMessage[0]["avatar"];

        if (itemNumber == "0"){
            updateText(displayName, "Item1Text2");
            StartCoroutine(loadImage("0", avatarUrl));
        }else if (itemNumber == "1"){
            updateText(displayName, "Item2Text2");
            StartCoroutine(loadImage("1", avatarUrl));
        }else if (itemNumber == "2"){
            updateText(displayName, "Item3Text2");
            StartCoroutine(loadImage("2", avatarUrl));
        }else if (itemNumber == "3"){
            updateText(displayName, "Item4Text2");
            StartCoroutine(loadImage("3", avatarUrl));
        }else if (itemNumber == "4"){
            updateText(displayName, "Item5Text2");
            StartCoroutine(loadImage("4", avatarUrl));
        }
    }


    void updateText(string textMsg, string itemTag){
        Item1Text1 = GameObject.FindWithTag(itemTag);
        myText = Item1Text1.GetComponent<Text>();
        myText.text = textMsg;
    }



	IEnumerator loadImage (string itemNumber, string myUrl) {
        Debug.Log("Loading image....");

        yield return 0;
        WWW imgLink = new WWW(myUrl);
        yield return imgLink;
        Debug.Log("Think I have the downloaded image");
        if(itemNumber == "0"){Avatar1.sprite = Sprite.Create(imgLink.texture, new Rect(0, 0, imgLink.texture.width, imgLink.texture.height), new Vector2(0, 0));}
        if(itemNumber == "1"){Avatar2.sprite = Sprite.Create(imgLink.texture, new Rect(0, 0, imgLink.texture.width, imgLink.texture.height), new Vector2(0, 0));}
        if(itemNumber == "2"){Avatar3.sprite = Sprite.Create(imgLink.texture, new Rect(0, 0, imgLink.texture.width, imgLink.texture.height), new Vector2(0, 0));}
        if(itemNumber == "3"){Avatar4.sprite = Sprite.Create(imgLink.texture, new Rect(0, 0, imgLink.texture.width, imgLink.texture.height), new Vector2(0, 0));}
        if(itemNumber == "4"){Avatar5.sprite = Sprite.Create(imgLink.texture, new Rect(0, 0, imgLink.texture.width, imgLink.texture.height), new Vector2(0, 0));}

        Item1Text1.GetComponent<Image>().sprite = Avatar1.sprite;
	}
}
