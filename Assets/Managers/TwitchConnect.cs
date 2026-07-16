using System.Collections;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System.IO;




public class TwitchConnect : MonoBehaviour
{
    public static TwitchConnect Instance;


    TcpClient Twitch;
    StreamReader Reader;
    StreamWriter Writer;
    ConcurrentQueue<(string user, string message)> chatQueue = new ConcurrentQueue<(string, string)>();

    const string URL = "irc.chat.twitch.tv";
    const int PORT = 6667;

    string User;
    //Get OAuth from https://twitchapps.com.tmi
    string OAuth;
    string Channel => User;

    float PingCounter = 0;

    public bool GameplayActive { get; private set; } = false;

    

    private void ConnectToTwitch()
    {
        Twitch = new TcpClient(URL, PORT);
        Reader= new StreamReader(Twitch.GetStream());
        Writer = new StreamWriter(Twitch.GetStream());

        Writer.WriteLine("PASS " + OAuth);
        Writer.WriteLine("NICK " + User.ToLower());
        Writer.WriteLine("JOIN #" + Channel.ToLower());
        Writer.Flush();

        
    }


    

    private void Awake()
    {
        Instance = this;

        StartCoroutine(InitializeLogin());
    }

    IEnumerator InitializeLogin()
    {
        while (DeviceCodeAuth.Instance == null)
            yield return null;

        while (string.IsNullOrEmpty(DeviceCodeAuth.Instance.Username))
            yield return null;

        User=DeviceCodeAuth.Instance.Username;

        OAuth = "oauth:" + PlayerPrefs.GetString("TwitchAccessToken");

        Debug.Log($"Connecting as {User}");

        StartConnectionLoop();
    }





    // Update is called once per frame
    void Update()
    {
        if(Twitch==null||!Twitch.Connected||Writer==null)
            return;

        PingCounter += Time.deltaTime;
        if (PingCounter > 60)
        {
            try
            {
                Writer.WriteLine("PING " + URL);
                Writer.Flush();
            }
            catch { }
            PingCounter = 0f;
        }

        if (!Twitch.Connected)
        {
            ConnectToTwitch();
        }

        if (GameplayActive)
        {
            while(chatQueue.TryDequeue(out var entry))
            {
                HandleChatMessage(entry.user,entry.message);
            }
        }
    }

    public void StartGameplay()
    {
        GameplayActive = true;
    }

    public void StopGameplay()
    {
        GameplayActive=false;

        while(chatQueue.TryDequeue(out _))
        {
            //Clear queued chat messages
        }
    }


    void ProcessMessage(string message)
    {
        

        if (message.StartsWith("PING"))
        {
            Writer.WriteLine("PONG :tmi.twitch.tv");
            Writer.Flush();
            return;
        }

        if (message.Contains("PRIVMSG"))
        {
            string[] split = message.Split('!');
            string user = split[0].Substring(1);

            int msgIndex = message.IndexOf("PRIVMSG");
            string chatMessage = message.Substring(message.IndexOf(':', msgIndex) + 1);

            // Add to queue for Unity main thread to handle
            chatQueue.Enqueue((user, chatMessage));
        }
    }

    void SendChatMessage(string msg)
    {
        Writer.WriteLine($"PRIVMSG #{Channel.ToLower()} :{msg}");
        Writer.Flush();
    }

    void HandleChatMessage(string user, string chatMessage)
    {
        if (string.IsNullOrWhiteSpace(chatMessage))
            return;

        // Take only the first word
        string[] parts = chatMessage.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 1)
            return;

        string firstWord = parts[0];

        const int MAX_WORD_LENGTH = 20;

        //Reject anything that isn't letters
        if (!System.Text.RegularExpressions.Regex.IsMatch(firstWord, "^[a-zA-Z]+$"))
        {
            SendChatMessage($"@{user} Only letters are allowed.");
            return;
        }

        //Reject overly long words
        if (firstWord.Length > MAX_WORD_LENGTH)
        {
            SendChatMessage($"@{user} Your word is too long. Maximum length is {MAX_WORD_LENGTH} letters.");
            return;
        }

        //Word is valid
        EnemySpawner.instance.QueueEnemy(firstWord, user);
    }

    void ListenLoop()
    {
        while (Twitch != null && Twitch.Connected)
        {
            try
            {
                if (Twitch.Available > 0)
                {
                    string message = Reader.ReadLine();
                    ProcessMessage(message);
                }
            }
            catch
            {
                break; // exit loop -> trigger reconnection
            }

            Thread.Sleep(5);
        }

        throw new System.Exception("Disconnected!");
    }

    void StartConnectionLoop()
    {
        new System.Threading.Thread(() =>
        {
            while (true)
            {
                try
                {
                    Debug.Log("Connecting to Twitch...");
                    ConnectToTwitch();
                    ListenLoop(); // <- this blocks until disconnection
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning("Connection lost. Reconnecting in 5 seconds...");
                    Thread.Sleep(5000);
                }
            }
        })
        { IsBackground = true }.Start();
    }

}
