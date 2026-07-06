using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;




public class TwitchConnect : MonoBehaviour
{
    TcpClient Twitch;
    StreamReader Reader;
    StreamWriter Writer;
    ConcurrentQueue<(string user, string message)> chatQueue = new ConcurrentQueue<(string, string)>();

    const string URL = "irc.chat.twitch.tv";
    const int PORT = 6667;

    string User;
    //Get OAuth from https://twitchapps.com.tmi
    string OAuth;
    string Channel;

    float PingCounter = 0;

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


    void LoadConfig()
    {
        string path = Path.Combine(Application.dataPath, "Managers", "TwitchConfig.json");

        if (!File.Exists(path))
        {
            Debug.LogError($"Config file not found: \n{path}");
            return;
        }

        string json=File.ReadAllText(path);

        TwitchConfig config=JsonUtility.FromJson<TwitchConfig>(json);

        User = config.username;
        OAuth=config.oauth;
        Channel = config.channel;
    }

    private void Awake()
    {
        LoadConfig();
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

        //if (Twitch.Available > 0)
        //{
        //    string message = Reader.ReadLine();
        //    //Debug.Log(message);

        //    if (message.Contains("PRIVMSG"))
        //    {
        //        string[] split = message.Split('!');
        //        string user = split[0].Substring(1);

        //        int msgIndex = message.IndexOf("PRIVMSG");
        //        string chatMessage = message.Substring(message.IndexOf(':', msgIndex) + 1);

        //        Debug.Log($"{user}: {chatMessage}");



        //        // COMMAND: !spawn <word>
        //        //if (chatMessage.StartsWith("!spawn"))
        //        //{
        //        //    string[] parts = chatMessage.Split(' ');

        //        //    if (parts.Length > 1)
        //        //    {
        //        //        string spawnWord = parts[1]; // Only the first word after !spawn
        //        //        EnemySpawner.instance.SpawnEnemyWithWord(spawnWord, user);
        //        //        Debug.Log($"Spawning enemy with word: {spawnWord} by {user}");
        //        //    }
        //        //}



        //        //Any Chat message spawns an enemy using the FIRST word
        //        // CHAT SPAWNS ENEMY: Only first word, and only letters A-Z
        //        if (!string.IsNullOrWhiteSpace(chatMessage))
        //        {
        //            string[] parts = chatMessage.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        //            if (parts.Length >= 1)
        //            {
        //                string firstWord = parts[0];

        //                // Allow only alphabetic words (case-insensitive)
        //                if (Regex.IsMatch(firstWord, "^[a-zA-Z]+$"))
        //                {
        //                    EnemySpawner.instance.SpawnEnemyWithWord(firstWord, user);
        //                    Debug.Log($"Spawned valid enemy -> Word: {firstWord}, User: {user}");
        //                }
        //                else
        //                {
        //                    Debug.Log($"Invalid word ignored: {firstWord}");

        //                    // Reply back to Twitch chat
        //                    SendChatMessage($"@{user} Only alphabet characters A-Z allowed!");
        //                }
        //            }
        //        }

        //    }

        //    if (message.StartsWith("PING"))
        //    {
        //        Writer.WriteLine("PONG: irc.chat.twitch.tv");
        //        Writer.Flush();
        //    }
            
        //}

        while(chatQueue.TryDequeue(out var entry))
        {
            HandleChatMessage(entry.user, entry.message);
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

        // Only allow alphabet characters
        if (System.Text.RegularExpressions.Regex.IsMatch(firstWord, "^[a-zA-Z]+$"))
        {
            EnemySpawner.instance.SpawnEnemyWithWord(firstWord, user);
        }
        else
        {
            // Optional: Send message back to Twitch about invalid words
            // SendChatMessage($"@{user} Only alphabet characters A-Z allowed!");
        }
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

            Thread.Sleep(10);
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
