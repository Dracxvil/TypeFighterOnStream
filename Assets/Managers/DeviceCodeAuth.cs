using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class DeviceCodeResponse
{
    public string device_code;
    public string user_code;
    public string verification_uri;
    public string verification_uri_complete;
    public int expires_in;
    public int interval;
}

[Serializable]
public class DeviceTokenResponse
{
    public string access_token;
    public string refresh_token;
    public string token_type;
    public int expires_in;
    public string[] scope;
}

[Serializable]
public class DeviceErrorResponse
{
    public string status;
    public string message;
}

[Serializable]
public class ValidateTokenResponse
{
    public string client_id;
    public string login;
    public string[] scopes;
    public string user_id;
    public int expires_in;
}

public class DeviceCodeAuth: MonoBehaviour
{
    public static DeviceCodeAuth Instance;

    [SerializeField] private string clientID = "";

    private const string scopes = "chat:read chat:edit";

    private string accessToken;
    private string refreshToken;

    public string Username {  get; private set; }

    public Action<DeviceCodeResponse> OnDeviceCodeReceived;

    public Action<DeviceTokenResponse> OnLoginSuccess;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void BeginDeviceLogin()
    {
        StartCoroutine(RequestDeviceCode());
    }

    IEnumerator RequestDeviceCode()
    {
        WWWForm form=new WWWForm();

        form.AddField("client_id", clientID);
        form.AddField("scopes", scopes);

        using UnityWebRequest req =
            UnityWebRequest.Post("https://id.twitch.tv/oauth2/device", form);

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(req.downloadHandler.text);
            yield break;
        }

        DeviceCodeResponse response =
            JsonUtility.FromJson<DeviceCodeResponse>(
                req.downloadHandler.text);

        Debug.Log("Device Code received!");

        Debug.Log("User code: " + response.user_code);

        Debug.Log("Verification URL: " + response.verification_uri);

        Application.OpenURL(response.verification_uri);

        OnDeviceCodeReceived?.Invoke(response);

        StartCoroutine(PollForAuthorization(response));
    }

    IEnumerator PollForAuthorization(DeviceCodeResponse response)
    {
        while (true)
        {
            WWWForm form = new WWWForm();

            form.AddField("client_id", clientID);
            form.AddField("device_code", response.device_code);
            form.AddField("grant_type",
                "urn:ietf:params:oauth:grant-type:device_code");

            using UnityWebRequest req =
                UnityWebRequest.Post("https://id.twitch.tv/oauth2/token",
                form);

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(req.downloadHandler.text);

                DeviceTokenResponse token=JsonUtility.FromJson<DeviceTokenResponse>(
                    req.downloadHandler.text);

                accessToken = token.access_token;
                refreshToken = token.refresh_token;

                SaveTokens();

                yield return StartCoroutine(ValidateToken());

                Debug.Log("Successfully authenticated");

                OnLoginSuccess?.Invoke(token);

                yield break;
            }

            string responseText = req.downloadHandler.text;

            Debug.Log(responseText);

            if (responseText.Contains("authorization_pending"))
            {
                Debug.Log("Waiting for user authorization...");
            }
            else if (responseText.Contains("slow_down"))
            {
                Debug.Log("Polling too quickly");
            }
            else if (responseText.Contains("expired_token"))
            {
                Debug.LogError("Device Code expired");

                yield break;
            }
            else
            {
                Debug.LogError(responseText);
                
                yield break;
            }
        }
    }

    void SaveTokens()
    {
        PlayerPrefs.SetString("TwitchAccessToken", accessToken);
        PlayerPrefs.SetString("TwitchRefreshToken", refreshToken);

        PlayerPrefs.Save();

        Debug.Log("Tokens saved.");
    }

    IEnumerator ValidateToken()
    {
        UnityWebRequest req =
            UnityWebRequest.Get("https://id.twitch.tv/oauth2/validate");

        req.SetRequestHeader("Authorization", "OAuth " + accessToken);

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(req.downloadHandler.text);
            yield break;
        }

        Debug.Log(req.downloadHandler.text);

        ValidateTokenResponse response = JsonUtility.FromJson<ValidateTokenResponse>(req.downloadHandler.text);

        Username = response.login;

        Debug.Log("Logged in as " + Username);

        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.SetConnectionStatus(Username);
        }
    }

    public void Logout()
    {
        PlayerPrefs.DeleteKey("TwitchAccessToken");
        PlayerPrefs.DeleteKey("TwitchRefreshToken");

        PlayerPrefs.Save();

        Username = "";

        accessToken = "";
        refreshToken = "";

        Debug.Log("Logged out");
    }

}