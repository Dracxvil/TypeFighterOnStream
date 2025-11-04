using UnityEngine;
using System.Net.Sockets;
using System.IO;


public class TwitchConnect : MonoBehaviour
{
    TcpClient Twitch;
    StreamReader Reader;
    StreamWriter Writer;

    const string URL = "irc.chat.twitch.tv";
    const int PORT = 6667;

    string User = "Dracxvil";
    //Get OAuth from https://twitchapps.com.tmi
    string OAuth = "oauth:36363pesiy08cudclwdanlmp3pvgct";
    string Channel = "Dracxvil";

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
        ConnectToTwitch();
    }
    // Update is called once per frame
    void Update()
    {
        if (Twitch.Available > 0)
        {
            string message = Reader.ReadLine();

            print(message);
        }
    }
}
