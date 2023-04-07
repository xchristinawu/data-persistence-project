using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public GameObject GameOverText;
    public Text BestPlayerInfo;

    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    public static string BestPlayer;
    public static int BestScore;

    private void Awake()
    {
        LoadScore();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
        
        SetBestPlayer();
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        PlayerData.Instance.playerScore = m_Points;
        m_GameOver = true;
        CheckBestPlayer();
        GameOverText.SetActive(true);
    }

    [System.Serializable]
    class SaveData
    {
        public string bestPlayer;
        public int bestScore;
    }

    void SaveScore(string name, int score)
    {
        SaveData data = new SaveData();

        data.bestPlayer = name;
        data.bestScore = score;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savescore.json", json);
    }

    void LoadScore()
    {
        string path = Application.persistentDataPath + "/savescore.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            BestPlayer = data.bestPlayer;
            BestScore = data.bestScore;
        }
    }

    void CheckBestPlayer()
    {
        int currentScore = PlayerData.Instance.playerScore;

        if (currentScore > BestScore)
        {
            BestPlayer = PlayerData.Instance.playerName;
            BestScore = currentScore;

            BestPlayerInfo.text = $"Best Score - {BestPlayer} : {BestScore}";
            
            SaveScore(BestPlayer, BestScore);
        }
    }

    void SetBestPlayer()
    {
        if (BestPlayer == null && BestScore == 0)
        {
            BestPlayerInfo.text = "";
        }
        else
        {
            BestPlayerInfo.text = $"Best Score - {BestPlayer} : {BestScore}";
        }
    }

}
