using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class leaderbords : MonoBehaviour
{

    public string[] playerName;
    public string[] playerScore;
    private int allRecords; // сколько всего записей
    private controller camera;
    public string data, debugSending, sendScore, userName, urlLb = "http://www.penguin-lab.online/hackpassword/leaderbord.php";
    private bool splitText;
    public string[] response;

    private void Awake()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
    }

    public void Start()
    {
        SearchCamera();
        ConnectionServies();
    }

    void Update()
    {
        if (splitText)
        {
            response = data.Split("#"[0]);
            playerName = new string[10];
            playerScore = new string[10];
            for (int i = 0; i < response.Length - 1; i++)
            {
                if (i < 10)
                {
                    string[] line = response[i].Split("&"[0]);
                    playerName[i] = line[0];
                    playerScore[i] = line[1];
                }
            }
            splitText = false;
            ShowLeaderbordText();
        }
    }

    public void SearchCamera()
    {
        camera = GameObject.FindWithTag("MainCamera").GetComponent<controller>();
    }

    public void ConnectionServies()
    {
        // авторизация на Google play servies
        Social.localUser.Authenticate((bool success) =>
        {
            // если подключение успешное
            if (success)
            {
                userName = Social.localUser.userName;//получаем имя игрока
            }
            else
            {

            }

        });
    }

    // публикая рекорда в таблицу лидеров

    public void PublishScore(int score)
    {
        sendScore = score + "";
        StartCoroutine("ConnectLeaderbord");
    }

    IEnumerator ConnectLeaderbord()
    {
        // создание новой формы
        WWWForm form = new WWWForm();
        // добавление в форму необходимых полей
        form.AddField("PlayerName", userName);
        form.AddField("PlayerScore", sendScore);
        // отправка формы
        WWW www = new WWW(urlLb, form);
        // ожиданеие и получения результата от работы php скрипта
        yield return www;
        debugSending = www.text;
        if (debugSending == "data received")
        {

        }
    }

    IEnumerator ShowLeaderbord()
    {
        camera.textLoading.SetActive(true);
        camera.GetComponent<Animator>().Play("HighScore");
        // отправка формы
        WWW www = new WWW(urlLb);
        // ожиданеие и получения результата от работы php скрипта
        yield return www;
        data = www.text;
        if (data == "error")
        {
            camera.textLoading.SetActive(false);
            camera.errorLoading.SetActive(true);
        }
        splitText = true;
    }

    void ShowLeaderbordText()
    {


        for (int i = 0; i < 10; i++)
        {
            camera.playerNameText[i].text = playerName[i];
            camera.playerScoreText[i].text = playerScore[i];
        }
        StartCoroutine("GetPlayerScore");
        camera.textLoading.SetActive(false);
        camera.scores.SetActive(true);
    }

    IEnumerator GetPlayerScore()
    {
        if (userName != "")
        {
            camera.errorUserName.SetActive(false);
            Debug.Log("start");
            WWWForm form = new WWWForm();
            // добавление в форму необходимых полей
            form.AddField("GetPlayerScore", userName);
            // отправка формы
            WWW www = new WWW(urlLb, form);
            // ожиданеие и получения результата от работы php скрипта
            yield return www;
            Debug.Log(www.text);
            Debug.Log("end");
            string[] line = www.text.Split("&"[0]);
            camera.yourPosLeaderbord.text = (int.Parse(line[0]) + 1) + "";
            camera.yourNameLeaderbords.text = line[1];
            camera.yourScoreLeaderbords.text = line[2];
        }
        else
        {
            camera.errorUserName.SetActive(true);
        }
    }



    /*void OnGUI ( )
	{
		GUILayout.Label(userName);
		//GUILayout.Label(debugSending);
	}*/

}
