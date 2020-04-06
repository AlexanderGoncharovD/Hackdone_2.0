using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class controller : MonoBehaviour
{
    public int level;
    public float timer, timer2;
    public string password, inputPassword, comparePassword;
    public bool passwordIsCorrect, isHardGame, isEasyGame;
    public Text textPassword, textLevel, textScore, textHighScore, textScoreEnd, textHighScoreEnd, leaderbordName, yourPosLeaderbord, yourNameLeaderbords, yourScoreLeaderbords;
    public Text[] timerText, playerNameText, playerScoreText;
    public int numInputSymbol; // индекс вводимого символа
    public AudioSource lockS, clock;
    public Animator anim;
    public Transform target;
    private GameObject line;
    public GameObject prefLine, linesTutorial, touchTutorial;
    public GameObject[] buttons, circleOut;
    public Color green, red, white, lightRed;
    public string possibleNumbers;
    private string resetPossibleNumbers;
    public bool isStartedGame, isStartAnimButton;
    public GameObject levelController, scores, textLoading, errorLoading, isLevelController, soundOn, soundMute;
    public levelController save;
    public bool isMenu = true, isMuteSound = false;
    public Transform butttonPlay;
    public Dropdown difDropdown;
    public int pointScore = 10;
    public int attempts; // количество попытко ввода пароля
    public bool isFirstGame = false; // если игра запущена в первый раз
    public GameObject menuCanvas;
    public GameObject errorUserName;
    public Sprite[] backgroundsSprites;
    public SpriteRenderer backgroundRenderer;
    public Button[] backgroundObjects;
    public GameObject[] backgroundCheckmarks;
    public GameObject rateApp;
    public GameObject backgroundsLevel4;
    public Text gameOverText; // Ссылка на объект, которые отображает подбадривающие надписи после проигрыша
    [TextArea]
    public string[] highScoreText; // Текст, который показывается, когда игрок побивает свой рекорд
    [TextArea]
    public string[] normalGameOverText; // Текст, который показывается после оконччания таймера
    [TextArea]
    public string[] loseScoreText; // Текст, который показывается, когда результат очень плохой меньше 3-х уровней

    private RaycastHit hit;
    private Ray ray;
	private float size;
	private bool newSpawn;
	private Transform lineTr;
	private Transform curButton;
	private Vector3 curB, midB, nextB; // узнать находится ли кнопка между двумя кнопками
	private int curId = -1, midId = -1;
	public int difficult = 4;
	public char[] newSymb, usedSymb;
	private Gradient gradient;
    private GradientColorKey[] colorKey;
    private GradientAlphaKey[] alphaKey;
	private float fraction;
	private Vector3 startMarker, endMarker = new Vector3(0, -2.25f, 0);
	private bool isRedTimer;
	private ads ads;
	private bool isRestart;

    void Awake()
    {
        /*PlayerPrefs.SetInt("MaxLvl", 0);
        PlayerPrefs.SetInt("BackgroundID", 0);
        PlayerPrefs.SetInt("backgroundsLevel4", 0);
        PlayerPrefs.SetInt("backgroundsLevel8", 0);
        PlayerPrefs.SetInt("AllNumberGame", 0);
        PlayerPrefs.SetInt("NoRateGooglePlay", 0);
        PlayerPrefs.SetInt("NumberNo", 0);*/

        backgroundRenderer.sprite = backgroundsSprites[PlayerPrefs.GetInt("BackgroundID")];
        backgroundCheckmarks[PlayerPrefs.GetInt("BackgroundID")].SetActive(true);
    }

	void Reset ( )
	{
		anim = GameObject.FindWithTag("AnimationController").GetComponent<Animator>( );
		target = GameObject.FindWithTag("Target").transform;
		green = new Color(0.0f, 0.699f, 0.150f, 1.0f);
		red = new Color(0.838f, 0.0f, 0.0f, 1.0f);
		white = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		possibleNumbers = resetPossibleNumbers = "012345678";
		Debug.LogWarning("Done reset");
	}

	void Start ( )
	{
        if (PlayerPrefs.GetInt("isFirstGame") == 0)
			isFirstGame = true;
		resetPossibleNumbers = possibleNumbers;
		isLevelController = GameObject.FindWithTag("LevelController");
		if (isLevelController == null)
			isLevelController = (GameObject)Instantiate(levelController);
		isLevelController.GetComponent<leaderbords>( ).SearchCamera ( );
		save = isLevelController.GetComponent<levelController>( );
		ads = isLevelController.GetComponent<ads>( );
		if (PlayerPrefs.GetInt("isMuteSound") == 0)
		{
			soundOn.SetActive (true);
			soundMute.SetActive (false);
			lockS.mute = clock.mute = isLevelController.GetComponent<AudioSource>( ).mute = false;
		}
		else
		{
			soundOn.SetActive (false);
			soundMute.SetActive (true);
			lockS.mute = clock.mute = isLevelController.GetComponent<AudioSource>( ).mute = true;
		}
        if (save.isMenu)
        {
            anim.Play("arrow");
            if (PlayerPrefs.GetInt("AllNumberGame") >= (PlayerPrefs.GetInt("NumberNo") + 1) * 3)
            {
                if (PlayerPrefs.GetInt("NoRateGooglePlay") == 0)
                {
                    rateApp.SetActive(true);
                }
            }
        }
        else
        {
            anim.Play("loadLevel");
        }
		startMarker = butttonPlay.position;
		timer = save.timerPass + (isHardGame ? 1 : 0);
		level = save.lvl;
		textLevel.text = "Уровень " + level;
		if (level <= 4)
			difficult = 4;
		else if (level <= 8)
			difficult = 5;
		else if (level <= 12)
			difficult = 6;
		else if (level <= 16)
			difficult = 7;
		else if (level <= 20)
			difficult = 8;
		else if (level > 20)
			difficult = 9;
		if (isFirstGame)
			password = "6452";
		else
			GenerationPassword (difficult);
		timerText[0].text = (int)(timer/10) + "";
		timerText[1].text = (int)(timer) - ((int)(timer/10))*10 + "";
		timerText[2].text = (int)((timer - Mathf.FloorToInt(timer))*10) + "";
		timerText[3].text = (int)((timer - Mathf.FloorToInt(timer))*100) - ((int)((timer - Mathf.FloorToInt(timer))*10))*10 + "";
		difDropdown.onValueChanged.AddListener(delegate {
					 DropdownValueChanged(difDropdown);
			 });
			difDropdown.value = PlayerPrefs.GetInt("Difficult");
		attempts = 1;
		textScore.text = "Очки: " + save.score;
		textHighScore.text = "Рекорд: " + PlayerPrefs.GetInt("HighScore");

        var curMaxLvl = PlayerPrefs.GetInt("MaxLvl");
        for (int i = 0; i < backgroundObjects.Length; i++)
        {
            if (curMaxLvl > 8 && curMaxLvl <= 16)
            {
                if (i < 6)
                {
                    backgroundObjects[i].interactable = true;
                }
            }
            else if (curMaxLvl > 16)
            {
                backgroundObjects[i].interactable = true;
            }
        }
        if(PlayerPrefs.GetInt("ShowBackgrounds") == 1)
        {
            PlayerPrefs.SetInt("ShowBackgrounds", 0);
            ShowSelectBackground();
        }
	}

	void Update ( )
	{
		if (save.isMenu)
		{
			if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
			{
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		    if (Physics.Raycast(ray, out hit, 20))
				{
					if (hit.collider.tag == "Play")
					{
			      butttonPlay.position = new Vector3(hit.point.x, butttonPlay.position.y, butttonPlay.position.z);
						if (Mathf.Abs(butttonPlay.position.x) > 1.8f)
						{
							anim.Play("startGame");
							lockS.Play ( );
							save.isMenu = false;
						}
						else
						{
							if (Input.GetMouseButtonUp(0))
							{
								anim.Play("startGame");
								save.isMenu = false;
							}
						}
					}
		    }
			}
			else
			{
				if (Input.GetMouseButtonUp(0))
				{
					fraction = 0;
					startMarker = butttonPlay.position;
				}
				fraction += Time.deltaTime*3;
				butttonPlay.position = Vector3.Lerp (startMarker, endMarker, fraction);
			}
		}
		else
		{
			if (Input.GetMouseButtonDown(0) && ! isStartedGame && ! isStartAnimButton)
			{
				isStartAnimButton = true;
				StartCoroutine ("VisiblePassword");
			}
			else if (Input.GetMouseButtonUp(0))
			{
				if (isStartedGame)
					if (! passwordIsCorrect)
						ResetLines ( );
			}

			if (isStartedGame && ! isFirstGame)
			{
				if (timer > 0)
				{
					if (timer <= 2.0f && ! isRedTimer)
						TimerColorRed ( );
					if (!passwordIsCorrect)
						timer -= Time.deltaTime;
					timerText[0].text = (int)(timer/10) + "";
					timerText[1].text = (int)(timer) - ((int)(timer/10))*10 + "";
					timerText[2].text = (int)((timer - Mathf.FloorToInt(timer))*10) + "";
					timerText[3].text = (int)((timer - Mathf.FloorToInt(timer))*100) - ((int)((timer - Mathf.FloorToInt(timer))*10))*10 + "";
				}
				else
				{
					if (timer != -1)
					{
						anim.Play("timeout");
						if (PlayerPrefs.GetInt("isMuteSound") == 0)
						{
							clock.Play ( );
							Handheld.Vibrate();
						}
						isStartedGame = false;
						textScoreEnd.text = textScore.text;
                        if (PlayerPrefs.GetInt("MaxLvl") < level)
                        {
                            PlayerPrefs.SetInt("MaxLvl", level);
                            if (level > 8)
                            {
                                if (PlayerPrefs.GetInt("backgroundsLevel4") == 0)
                                {
                                    PlayerPrefs.SetInt("backgroundsLevel4", 1);
                                    backgroundsLevel4.SetActive(true);
                                }
                                if (level > 16)
                                {
                                    if (PlayerPrefs.GetInt("backgroundsLevel8") == 0)
                                    {
                                        PlayerPrefs.SetInt("backgroundsLevel8", 1);
                                        backgroundsLevel4.SetActive(true);
                                    }
                                }
                            }
                        }
						if (save.score >= PlayerPrefs.GetInt("HighScore"))
						{
							PlayerPrefs.SetInt("HighScore", save.score);
							isLevelController.GetComponent<leaderbords>( ).PublishScore (save.score);
                            gameOverText.text = highScoreText[Random.Range(0, highScoreText.Length)];
						}
                        else
                        {
                            if(level <= 3)
                            {
                                gameOverText.text = loseScoreText[Random.Range(0, loseScoreText.Length)];
                            }
                            else
                            {
                                gameOverText.text = normalGameOverText[Random.Range(0, normalGameOverText.Length)];
                            }
                        }
                        PlayerPrefs.SetInt("AllNumberGame", PlayerPrefs.GetInt("AllNumberGame") + 1);
						textHighScoreEnd.text = "Рекорд: " + PlayerPrefs.GetInt("HighScore");
						if (line != null)
							Destroy (line);
						timer = -1;
						timer = 0.0f;
						timerText[0].text = timerText[1].text = timerText[2].text = timerText[3].text = "0";
					}
				}
			}

		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	    if (Physics.Raycast(ray, out hit, 20))
	      target.position = hit.point;

			if (line != null && ! newSpawn)
			{
				lineTr = line.transform;
				size = Vector3.Distance(lineTr.position, target.position);
				lineTr.localScale = new Vector3(0, 0, size);
				lineTr.LookAt(target);
			}
		}

		if (Input.GetKeyDown(KeyCode.Escape))
			BackButton ( );
	}

	public void CreateLine (Transform newPos, int id)
	{
		if (isFirstGame)
			touchTutorial.GetComponent<SpriteRenderer>( ).enabled = false;
		inputPassword += id + "";
		numInputSymbol++;
		InputXpassword (numInputSymbol);

		comparePassword = password;
		comparePassword = password.Substring(0, numInputSymbol); // Обрезать строку по количество введенных символов
        // Если введенный символы совпадают с генерированным паролем, то отмечаем точку зеленым, иначе - красным
		if (comparePassword == inputPassword)
		{
			SetColor (green, id);
			if (numInputSymbol == difficult)
			{
				passwordIsCorrect = true;
				textPassword.color = green;
				StartCoroutine(NextLevel ( ));
			}
		}
		else
		{
			SetColor (red, id);
			if (numInputSymbol == difficult)
			{
				textPassword.color = red;
			}
		}
        // генерируем новую линю
		newSpawn = true;
        // проверка первого нажатия, если id выходит из массива точек, то это первое нажатие 
		if (curId == -1)
			curId = id;
        // Если новой линии не существует 
		if (line == null)
        {
            // Создаем новую линию из этой точки
            line = (GameObject)Instantiate(prefLine, newPos.position, Quaternion.Euler(0, 90, 0));
            curButton = newPos;
        }
        else // Если линия уже существует, то есть мы ведем палец уже к второй точке
		{
            /* Проверям, существует ли между начальной и конечной точкой еще одна точка
            Проверяется по формуле: складываем id первой и id последней точки, если их сумма делится на два без остатка,
            то по середине лежит точка и она нам подохид*/
			if ((curId+id)%2 == 0)
			{
                // Дополнительно проверям дистанцию
				if (Vector3.Distance(curButton.position, newPos.position) > 2.6f)
				{
					midId = (curId+id) / 2; // получаем id средней точки
					if (midId != curId && midId != id)
					{
                        // получаем позицию, всех трех точек, по id из массива
						curB = buttons[curId].transform.position;
						midB = buttons[midId].transform.position;
						nextB = buttons[id].transform.position;
                        // проверям лежит ли средняя точка на прямой, по формуле:
						if ((midB.x - curB.x) * (nextB.y - curB.y) - (nextB.x - curB.x) * (midB.y - curB.y) == 0)
						{
                            // если средняя точка свободна (еще не учавствовала в вводе пароля)
							if (! buttons[midId].GetComponent<button>( ).isBusy)
							{
								Create (newPos, id); // создать линию
                                // занимаем "резервируем" среднюю точку
								buttons[midId].GetComponent<button>( ).isBusy = true;
								inputPassword += id + ""; // добавляем id в вводимый пароль
								numInputSymbol++; // на один введенный символ стало болльше
								InputXpassword (numInputSymbol); // Функция вывод на экран количества введенных символов
								comparePassword = password;
                                // Обрезать строку по количество введенных символов
                                comparePassword = password.Substring(0, numInputSymbol);
                                // перекраска в зеленый, если цифры в пароле верные, иначе - в красный
								if (comparePassword == inputPassword)
								{
									SetColor (green, midId);
									if (numInputSymbol == difficult)
									{
										passwordIsCorrect = true;
										textPassword.color = green;
										Destroy(line);
										StartCoroutine(NextLevel ( ));
									}
								}
								else
								{
									SetColor (red, midId);
									if (numInputSymbol == difficult)
									{
										textPassword.color = red;
										Destroy(line);
									}
								}
							}
						}
						else
						{
                            // если средняя точка не лежит на прямой, то создаем линию не учитывая ее
							if (! buttons[id].GetComponent<button>( ).isBusy)
							{
								Create (newPos, id); // создать линию
                                buttons[id].GetComponent<button>( ).isBusy = true;
							}
						}
					}
					else
						Create (newPos, id); // создать линию
                }
				else
					Create (newPos, id); // создать линию
            }
			else
				if (! buttons[id].GetComponent<button>( ).isBusy)
					Create (newPos, id); // создать линию
        }
		newSpawn = false;
	}

    // создать линию
    void Create (Transform newPos, int id)
	{
		buttons[id].GetComponent<button>( ).isBusy = true; // занимаем "резервируем" среднюю точку
        lineTr.LookAt(newPos); // поворачиваем линию в сторону новой точки
		size = Vector3.Distance(curButton.position, newPos.position); // определяем длину лини между точками
		lineTr.localScale = new Vector3(0, 0, size);
        // Если количество введенных символов меньше обще длины пароля
		if (numInputSymbol < difficult)
			line = (GameObject)Instantiate(prefLine, newPos.position, Quaternion.Euler(0, 90, 0));
		else
			line = null; // иначе завершаем создание новых линий
		curButton = newPos;
		curId = id;
	}

	void GenerationPassword (int number)
	{
		var rnd = 0; // номер случайного символа в строке
		var symbol = ""; // случайный символ
		password = ""; // получаемый пароль в результате работы алгоритма
		textPassword.text = ""; // для вывода на экран длины пароля
		// цикл генерирующий пароль нужной длины
		for (int i = 0; i < number; i++)
		{
			textPassword.text += "- "; // добавляем прочерк к строке, как обозначеие символа
			newSymb = possibleNumbers.ToCharArray ( ); // конвертируес string в Char как массив символов
			usedSymb = password.ToCharArray ( ); // создаем массив из уже использованных символов в пароле
			rnd = Random.Range(0, newSymb.Length); // выбираем случайный символ из массива доступных
			symbol = newSymb[rnd]+""; // присвоение символа в переменную типа string
			password += symbol+""; // добавляем этот символ в пароль
			possibleNumbers = resetPossibleNumbers; // восстанавливаем последовательноть доступных числе до первоначального вида (012345678)
			// если случайно выбранный символ 0
			if (symbol == "0")
			{
				// то удаляем из строки всех символов "недоступные" символы
				possibleNumbers = possibleNumbers.Replace("0", "");
				possibleNumbers = possibleNumbers.Replace("6", "");
				possibleNumbers = possibleNumbers.Replace("8", "");
				possibleNumbers = possibleNumbers.Replace("2", "");
			}
			// если случайно выбранный символ 1
			else if (symbol == "1")
			{
				// то удаляем из строки всех символов "недоступные" символы
				possibleNumbers = possibleNumbers.Replace("1", "");
				possibleNumbers = possibleNumbers.Replace("7", "");
			}
			// если случайно выбранный символ 2
			else if (symbol == "2")
			{
				// то удаляем из строки всех символов "недоступные" символы
				possibleNumbers = possibleNumbers.Replace("2", "");
				possibleNumbers = possibleNumbers.Replace("0", "");
				possibleNumbers = possibleNumbers.Replace("6", "");
				possibleNumbers = possibleNumbers.Replace("8", "");
			}
			// если случайно выбранный символ 3
			else if (symbol == "3")
			{
				// то удаляем из строки всех символов "недоступные" символы
				possibleNumbers = possibleNumbers.Replace("3", "");
				possibleNumbers = possibleNumbers.Replace("5", "");
			}
			// если случайно выбранный символ 4
			else if (symbol == "4")
			{
				// то удаляем из строки всех символов "недоступные" символы
				possibleNumbers = possibleNumbers.Replace("4", "");
			}
			// если случайно выбранный символ 5
			else if (symbol == "5")
			{
				// то удаляем из строки всех символов "недоступные" символы
				possibleNumbers = possibleNumbers.Replace("5", "");
				possibleNumbers = possibleNumbers.Replace("3", "");
			}
			// если случайно выбранный символ 6
			else if (symbol == "6")
			{
				// то удаляем из строки всех символов "недоступные" символы
				possibleNumbers = possibleNumbers.Replace("6", "");
				possibleNumbers = possibleNumbers.Replace("0", "");
				possibleNumbers = possibleNumbers.Replace("2", "");
				possibleNumbers = possibleNumbers.Replace("8", "");
			}
			// если случайно выбранный символ 7
			else if (symbol == "7")
			{
				// то удаляем из строки всех символов "недоступные" символы
				possibleNumbers = possibleNumbers.Replace("7", "");
				possibleNumbers = possibleNumbers.Replace("1", "");
			}
			// если случайно выбранный символ 8
			else if (symbol == "8")
			{
				// то удаляем из строки всех символов "недоступные" символы
				possibleNumbers = possibleNumbers.Replace("8", "");
				possibleNumbers = possibleNumbers.Replace("0", "");
				possibleNumbers = possibleNumbers.Replace("2", "");
				possibleNumbers = possibleNumbers.Replace("6", "");
			}
			// удаляем из оставшейся последовательности символов, символы уже содержащиеся в пароле
			for (int j = 0; j < usedSymb.Length; j++)
				possibleNumbers = possibleNumbers.Replace(usedSymb[j]+"", "");
		}
		// всё, по заверешению цикла, будет сгенерирован решаемый пароль
	}

	void SetColor (Color color, int id)
	{
		gradient = new Gradient();

		colorKey = new GradientColorKey[2];
		colorKey[0].color = color;
		colorKey[0].time = 0.0f;
		colorKey[1].color = color;
		colorKey[1].time = 1.0f;

		alphaKey = new GradientAlphaKey[2];
		alphaKey[0].alpha = 1.0f;
		alphaKey[0].time = 0.0f;
		alphaKey[1].alpha = 1.0f;
		alphaKey[1].time = 1.0f;

		gradient.SetKeys(colorKey, alphaKey);
		if (numInputSymbol > 1)
			line.GetComponent<LineRenderer>( ).colorGradient = gradient;
		buttons[id].GetComponent<SpriteRenderer>( ).color = color;
		circleOut[id].GetComponent<SpriteRenderer>( ).color = color;
		circleOut[id].SetActive (true);
	}

	void ResetLines ( )
	{
		//Debug.Log(int.Parse(password.Substring(0, password.Length-(difficult[0]-1))));
		for(int i = 0; i < buttons.Length; i++)
		{
			buttons[i].GetComponent<button>( ).isBusy = false;
			buttons[i].GetComponent<CircleCollider2D>( ).enabled = true;
			if (isEasyGame)
			{
				if (i == int.Parse(password.Substring(0, password.Length-(difficult-1))))
				{
					buttons[i].GetComponent<SpriteRenderer>( ).color = green;
					circleOut[i].GetComponent<SpriteRenderer>( ).color = green;
					circleOut[i].SetActive (true);
				}
				else {
					buttons[i].GetComponent<SpriteRenderer>( ).color = white;
					circleOut[i].GetComponent<SpriteRenderer>( ).color = white;
					circleOut[i].SetActive (false);
				}
			}
			else if (isHardGame)
			{
				buttons[i].GetComponent<SpriteRenderer>( ).color = white;
				circleOut[i].GetComponent<SpriteRenderer>( ).color = white;
				circleOut[i].SetActive (false);
			}
		}
		GameObject[] lines = GameObject.FindGameObjectsWithTag("Line");
		if (lines.Length > 0)
			attempts++;
		for(int i = 0; i < lines.Length; i++)
		{
			Destroy(lines[i]);
		}
		textPassword.color = white;
		textPassword.text = "";
		for(int i = 0; i < difficult; i++)
		{
			textPassword.text += "- ";
		}
		numInputSymbol = 0;
		comparePassword = inputPassword = "";

		if (isFirstGame)
		{
			anim.Play("touchTutorial");
			touchTutorial.GetComponent<SpriteRenderer>( ).enabled = true;
		}
	}

	IEnumerator VisiblePassword ( )
	{
		isStartAnimButton = true;
		usedSymb = password.ToCharArray ( );
		for(int i = 0; i < usedSymb.Length; i++)
		{
			circleOut[int.Parse(usedSymb[i]+"")].SetActive (true);
			SetColor (green, int.Parse(usedSymb[i]+""));
			if (isFirstGame)
				yield return new WaitForSeconds (0.7f);
			else
				yield return new WaitForSeconds (0.5f);
			if (isEasyGame)
			{
				if (i == 0 )
					SetColor (green, int.Parse(usedSymb[i]+""));
				else
					SetColor (white, int.Parse(usedSymb[i]+""));
					circleOut[int.Parse(usedSymb[i]+"")].SetActive (false);
			}
			else if (isHardGame)
			{
				SetColor (white, int.Parse(usedSymb[i]+""));
				circleOut[int.Parse(usedSymb[i]+"")].SetActive (false);
			}
		}
		if (isFirstGame)
		{
			linesTutorial.SetActive (true);
			anim.Play ("touchTutorial");
		}
		else
			anim.Play("go");
		isStartedGame = true;
	}

	void InputXpassword (int numberX)
	{
		textPassword.text = "";
		for (int i = difficult; i > 0; i--)
		{
			if (numberX != 0)
			{
				textPassword.text += "X ";
				numberX--;
			}
			else
				textPassword.text += "- ";
		}
	}

	IEnumerator NextLevel ( )
	{
		if (isFirstGame)
		{
			linesTutorial.SetActive (false);
			PlayerPrefs.SetInt("isFirstGame", 1);
		}
		lockS.Play ( );
		yield return new WaitForSeconds (0.3f);
		anim.Play("nextLevel");
		save.lvl++;
		save.timerPass += 0.25f;
		save.score += (pointScore * difficult) / attempts;
		textScore.text = "Очки: " + save.score;
		if (save.score >= PlayerPrefs.GetInt("HighScore"))
			PlayerPrefs.SetInt("HighScore", save.score);
		GameObject[] lines = GameObject.FindGameObjectsWithTag("Line");

		for(int i = 0; i < lines.Length; i++)
		{
			Destroy(lines[i]);
		}
	}

	void UpdateLevel ( )
	{
		SceneManager.LoadScene(0);
	}

	void TimerColorRed ( )
	{
		isRedTimer = true;
		timerText[0].color = timerText[1].color = timerText[2].color = timerText[3].color = timerText[4].color = lightRed;
	}

	public void BackButton ( )
	{
		save.isMenu = true;
		save.lvl = 1;
		save.timerPass = 5.0f;
		save.score = 0;
		SceneManager.LoadScene (0);
		if (isRestart)
			if (Random.Range(0.0f, 1.0f) <= 0.6f)
				ads.showIinterstitial = true;
	}

	public void RestartButton ( )
	{
		isRestart = true;
	}

	void DropdownValueChanged(Dropdown change)
  {
		if (change.value == 0)
		{
			isEasyGame = true;
			isHardGame = false;
			PlayerPrefs.SetInt("Difficult", 0);
		}
		else if (change.value == 1)
		{
			isEasyGame = false;
			isHardGame = true;
			PlayerPrefs.SetInt("Difficult", 1);
		}
  }

	public void ShowTutorial ( )
	{
		anim.Play("tutorial");
	}
	public void ShowHighScores ( )
	{
		isLevelController.GetComponent<leaderbords>( ).StartCoroutine("ShowLeaderbord");
	}

    public void ShowSelectBackground()
    {

        anim.Play("selectBackground");
    }

	public void MuteSound ( )
	{

		if (PlayerPrefs.GetInt("isMuteSound") == 0)
		{
			PlayerPrefs.SetInt("isMuteSound", 1);
			soundOn.SetActive (false);
			soundMute.SetActive (true);
			lockS.mute = clock.mute = isLevelController.GetComponent<AudioSource>( ).mute = true;
		}
		else
		{
			soundOn.SetActive (true);
			soundMute.SetActive (false);
			PlayerPrefs.SetInt("isMuteSound", 0);
			lockS.mute = clock.mute = isLevelController.GetComponent<AudioSource>( ).mute = false;
		}
	}

    public void SelectedBackground(int i)
    {
        backgroundRenderer.sprite = backgroundsSprites[i - 1];
        PlayerPrefs.SetInt("BackgroundID", (i - 1));
        for (int r = 0; r < backgroundCheckmarks.Length; r++)
        {
            backgroundCheckmarks[r].SetActive(false);
        }
        backgroundCheckmarks[i - 1].SetActive(true);
    }

    public void LookBackgrounds()
    {
        PlayerPrefs.SetInt("ShowBackgrounds", 1);
        BackButton();
    }

    public void OpenGooglePlay()
    {
        PlayerPrefs.SetInt("NoRateGooglePlay", 1);
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.PenguinLab.brain.training");
        rateApp.SetActive(false);
    }

    public void NoRateGooglePlay()
    {
        PlayerPrefs.SetInt("NoRateGooglePlay", 1);
        rateApp.SetActive(false);
    }
    public void NoRate()
    {
        rateApp.SetActive(false);
        PlayerPrefs.SetInt("NumberNo", PlayerPrefs.GetInt("NumberNo") + 1);
    }
}
