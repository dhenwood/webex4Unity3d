using System;
using System.Collections;
using UnityEngine;
using SimpleJSON;
using Vuplex.WebView;
using UnityEngine.Networking;

public class OAuthScript : MonoBehaviour
{
    string clientId = "<removed>"; // obtained from developer.webex.com when creating Integration
    string clientSecret = "<removed>"; // obtained from developer.webex.com when creating Integration
    string initUrl = "<removed>"; // obtained from developer.webex.com when creating Integration
    
    CanvasWebViewPrefab webViewPrefab;

    void Start()
    {
        Web.SetStorageEnabled(false); //The same as Incognito mode
        
        getAccessToken();
    }

    async void getAccessToken(){
        webViewPrefab = GameObject.Find("CanvasWebViewPrefab").GetComponent<CanvasWebViewPrefab>();
        await webViewPrefab.WaitUntilInitialized();

        webViewPrefab.WebView.LoadUrl(initUrl);

        webViewPrefab.WebView.UrlChanged += (sender, eventArgs) => {
            string response = eventArgs.Url;
            if (response.StartsWith("myapp://")) {
                int codeStart = response.IndexOf("myapp://wss?code=") + "myapp://wss?code=".Length;
                int codeFinish = response.LastIndexOf("&state=set_state_here");
                String myCode = response.Substring(codeStart, codeFinish - codeStart);
                Debug.Log("code: " + myCode);

                StartCoroutine(getFinalToken(myCode));
            }
        };
    }
    

    IEnumerator getFinalToken(string code){
        WWWForm form = new WWWForm();
        form.AddField("grant_type", "authorization_code");
        form.AddField("client_id", clientId);
        form.AddField("client_secret", clientSecret);
        form.AddField("code", code);
        form.AddField("redirect_uri", "myapp://wss");
                
        UnityWebRequest www = UnityWebRequest.Post("https://webexapis.com/v1/access_token", form);
        www.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        yield return www.SendWebRequest();
                
        if (www.result != UnityWebRequest.Result.Success){
            Debug.Log(www.error);
        }else{
            Debug.Log("ResponseForToken: " + www.downloadHandler.text);
            var json = JSON.Parse(www.downloadHandler.text);
            string token = json["access_token"];
            string expire = json["expires_in"];
            string refresh_token = json["refresh_token"];
            string refresh_token_expires_in = json["refresh_token_expires_in"];

            Debug.Log("accessToken: " + token);
            PlayerPrefs.SetString("token", token);
            webViewPrefab.WebView.LoadUrl("https://talk2spark.com/wssOauthRedirect.html");
        }
    }
}
