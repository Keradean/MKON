using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
//Hauk
public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;

    [Header("API Base URL (no trailing slash)")]
    [SerializeField] private string baseUrl = "http://localhost/MKON";
    public bool IsDatabaseOnline { get; private set; } = false;

    // Response structure for fetching times
    [Serializable]
    private class GetTimesResponse
    {
        public bool ok;
        public float raceTime;
        public float roundTime;
        public string error;
    }

    // Request structure for updating a time value
    [Serializable]
    private class UpdateTimeRequest
    {
        public string mapName;
        public float time;
        public string column; // "RaceTime" or "RoundTime"
    }

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Test database connection on startup
        StartCoroutine(TestDatabaseConnection());
        if (!IsDatabaseOnline)
        {
            Debug.LogError("Cannot perform further tests without a database connection.");
            return;
        }
        // Map used for testing
        string testMap = "DesertCity";

        Debug.Log("=== TEST: FetchTimes + UpdateTimes ===");

        // 1) Fetch current times from the database
        FetchTimes(testMap, (race, round) =>
        {
            Debug.Log($"[Before Update] {testMap} -> RaceTime: {race}, RoundTime: {round}");
            /*
            // 2) Define new test values to overwrite the database entries
            float newRace = 510.43f;
            float newRound = 150.43f;

            // Send update requests to the server
            UpdateRaceTime(testMap, newRace);
            UpdateRoundTime(testMap, newRound);

            Debug.Log($"Updating times for {testMap}...");
            */
        });

    }

    private IEnumerator TestDatabaseConnection()
    {
        string testMap = "DesertCity";

        Debug.Log("Testing database connection...");

        bool resultReceived = false;

        // Try fetching times
        FetchTimes(testMap, (race, round) =>
        {
            // If race < 0, FetchTimes already signaled an error
            if (race < 0)
            {
                IsDatabaseOnline = false;
                Debug.LogError("Database connection failed.");
            }
            else
            {
                IsDatabaseOnline = true;
                Debug.Log("Database connection OK.");
            }

            resultReceived = true;
        });

        // Wait until callback arrives
        while (!resultReceived)
            yield return null;
    }

    // FETCH TIMES FROM DATABASE
    public void FetchTimes(string mapName, Action<float, float> onResult)
    {
        StartCoroutine(FetchTimesCoroutine(mapName, onResult));
    }

    private IEnumerator FetchTimesCoroutine(string mapName, Action<float, float> onResult)
    {
        // Build URL with map name as GET parameter
        string url = $"{baseUrl}/get_times.php?mapName={UnityWebRequest.EscapeURL(mapName)}";

        using var req = UnityWebRequest.Get(url);

        // Send request to server
        yield return req.SendWebRequest();

        // Handle connection errors
        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("FetchTimes failed: " + req.error);
            onResult?.Invoke(-1, -1);
            yield break;
        }

        // Parse JSON response
        var res = JsonUtility.FromJson<GetTimesResponse>(req.downloadHandler.text);

        // Validate response
        if (res == null || !res.ok)
        {
            Debug.LogError("Server error: " + (res?.error ?? "Invalid JSON"));
            onResult?.Invoke(-1, -1);
            yield break;
        }

        // Return the two time values
        onResult?.Invoke(res.raceTime, res.roundTime);
    }

    // UPDATE TIME (RaceTime or RoundTime)
    public void UpdateRaceTime(string mapName, float newTime)
    {
        UpdateTime(mapName, newTime, "RaceTime");
    }

    public void UpdateRoundTime(string mapName, float newTime)
    {
        UpdateTime(mapName, newTime, "RoundTime");
    }

    private void UpdateTime(string mapName, float newTime, string column)
    {
        StartCoroutine(UpdateTimeCoroutine(mapName, newTime, column));
    }

    private IEnumerator UpdateTimeCoroutine(string mapName, float newTime, string column)
    {
        string url = $"{baseUrl}/save_time.php";

        // Create request object
        var reqObj = new UpdateTimeRequest
        {
            mapName = mapName,
            time = newTime,
            column = column
        };

        // Convert to JSON
        string json = JsonUtility.ToJson(reqObj);
        byte[] body = Encoding.UTF8.GetBytes(json);

        // Create POST request
        using var req = new UnityWebRequest(url, "POST");
        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        // Send request
        yield return req.SendWebRequest();

        // Handle errors
        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("UpdateTime failed: " + req.error);
            yield break;
        }

        // Log server response
        Debug.Log("UpdateTime response: " + req.downloadHandler.text);
    }
}
