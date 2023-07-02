using UnityEngine;
using NativeWebSocket;
public class GameMessage
{
    public string opcode;
    public string data;
    [SerializeField]
     string action = "sendmessage";
    public GameMessage(string opcode, string data)
    {
        this.opcode = opcode;
        this.data = data;
    }
}

public class GamePrivateMessage : GameMessage
{
    [SerializeField]
    readonly  string action = "sendwhisper";

    public string receiver;
    public GamePrivateMessage(string opcode, string receiver, string data) : base(opcode,  data)
    {
        this.receiver = receiver;
    }
}
public class WebSocketService : MonoBehaviour
{

    private static WebSocketService instance;

    //private Rigidbody localPlayerReference;
    //private bool intentionalClose = false;
    private WebSocket websocket;
    private string _webSocketDns = "wss://lihfkya272.execute-api.ap-southeast-1.amazonaws.com/Prod";


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

    }

    async void Start()
    {
        Debug.Log("Websocket start");

        websocket = new WebSocket(_webSocketDns);

        SetupWebsocketCallbacks();
        await websocket.Connect();
    }

    void Update()
    {


#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    // All messages received through the websocket connection are processed here
    private void ProcessReceivedMessage(string message)
    {
        GameMessage gameMessage = JsonUtility.FromJson<GameMessage>(message);

        //if (gameMessage.opcode == PlayingOp)
        //{
        //}
    }

    // Establishes the connection's lifecycle callbacks.
    // Once the connection is established, OnOpen, it automatically attempts to create or join a game through the RequestStartOp code.
    private void SetupWebsocketCallbacks()
    {
        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            GameMessage startRequest = new GameMessage(Constants.opcode_intro, Constants.GetRandomName());
            SendWebSocketMessage(JsonUtility.ToJson(startRequest));
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("OnMessage "+message.ToString());

            ProcessReceivedMessage(message);
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };
    }

    // Connects to the websocket
    async public void FindMatch()
    {
        // waiting for messages
        await websocket.Connect();
    }

    //private void SendVectorAsMessage(Vector3 vector, string opCode, int seq)
    //{
    //    SerializableVector3 posToSend = vector;
    //    GameMessage posMessage = new PlayerPositionMessage("OnMessage", opCode, posToSend, new SerializableVector3(), 0, seq, "", localPlayerReference.position);
    //    posMessage.uuid = matchId;
    //    SendWebSocketMessage(JsonUtility.ToJson(posMessage));
    //}


    public async void SendWebSocketMessage(string message)
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            Debug.Log("SendWebSocketMessage\n" + message);
            // Sending plain text
            await websocket.SendText(message);
        }
    }



    public async void QuitGame()
    {
        await websocket.Close();
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }



    public void init() { }

    protected WebSocketService() { }
}
