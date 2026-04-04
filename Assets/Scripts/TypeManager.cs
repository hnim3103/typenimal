using System.Text;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;

public class TypeManager : MonoBehaviour
{
    [System.Serializable]
    public class MongoResponse
    {
        public string text;
    }

    public TMP_Text targetTextDisplay;
    public TMP_InputField inputField;
    public TMP_Text wpmDisplay;

    public Animator playerAnimator;
    public MoveLeft backgroundScript;
    public MoveLeft groundScript;

    public float winMoveSpeed = 5f;

    private bool isMovingOffScreen = false;
    private bool hasWon = false;

    private string url = "https://typenimalbe.onrender.com/api/v1/documents/get-random-document";
    private string targetText = "";

    private Queue<string> targetWords;
    private List<string> completedWords = new List<string>();

    private bool isLoaded = false;

    [SerializeField] private CoundownTimer countdownTimer;

    private int totalTypedCharacters = 0;
    private float timeElapsed = 0f;
    private bool isTypingStarted = false;

    private Canvas gameOverCanvas;

    private void Start()
    {
        StartCoroutine(FetchRandomDocument());

        inputField.text = "";
        inputField.ActivateInputField();

        backgroundScript.enabled = false;
        groundScript.enabled = false;

        if (wpmDisplay != null) wpmDisplay.text = "0 WPM";

        inputField.onValueChanged.AddListener(OnTyping);

        UpdateDisplay("");
        gameOverCanvas = GameObject.Find("GameOverCanvas").GetComponent<Canvas>();
        gameOverCanvas.enabled = false;
    }

    IEnumerator FetchRandomDocument()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Database Error: " + webRequest.error);
                targetTextDisplay.text = "Failed to load text from database.";
            }
            else
            {
                string rawJson = webRequest.downloadHandler.text;

                MongoResponse response = JsonUtility.FromJson<MongoResponse>(rawJson);
                targetText = response.text;

                countdownTimer.StartTimer(targetText);

                targetWords = new Queue<string>(targetText.Split(' '));

                isLoaded = true;

                UpdateDisplay("");

                //Debug.Log("Game Ready! Text: " + targetText);
            }
        }
    }

    void OnTyping(string input)
    {
        if (!isLoaded || targetWords == null)
            return; 

        if (!isTypingStarted && input.Length > 0)
        {
            isTypingStarted = true;
        }

        if (input.Contains(" "))
        {
            HandleSpace(input);
            return;
        }

        UpdateDisplay(input);
    }

    void HandleSpace(string input)
    {
        if (targetWords == null || targetWords.Count == 0)
            return;

        string typedWord = input.Trim();
        string targetWord = targetWords.Peek();

        if (typedWord == targetWord)
        {
            CancelInvoke(nameof(StopGame));

            completedWords.Add(targetWords.Dequeue());
            totalTypedCharacters += typedWord.Length + 1; // +1 for the space

            MoveGameForward();

            inputField.text = "";
            inputField.caretPosition = 0;

            Invoke(nameof(StopGame), 1f);
        }
        else
        {
            inputField.text = typedWord;
            inputField.caretPosition = inputField.text.Length;

            StopGame();
        }

        UpdateDisplay(inputField.text);
    }

    void UpdateDisplay(string currentInput)
    {
        if (targetWords == null)
            return;

        StringBuilder result = new StringBuilder();

        foreach (string word in completedWords)
        {
            result.Append($"<color=#8BC34A>{word}</color> ");
        }

        if (targetWords.Count > 0)
        {
            string[] remainingWords = targetWords.ToArray();
            string currentWord = remainingWords[0];

            for (int j = 0; j < currentWord.Length; j++)
            {
                if (j < currentInput.Length)
                {
                    if (currentInput[j] == currentWord[j])
                        result.Append($"<color=#8BC34A>{currentWord[j]}</color>");
                    else
                        result.Append($"<color=#FF5252>{currentWord[j]}</color>");
                }
                else if (j == currentInput.Length)
                {
                    result.Append($"<u><color=#000000>{currentWord[j]}</color></u>");
                }
                else
                {
                    result.Append($"<color=#000000>{currentWord[j]}</color>");
                }
            }

            result.Append(" ");

            for (int i = 1; i < remainingWords.Length; i++)
            {
                result.Append($"<color=#000000>{remainingWords[i]}</color> ");
            }
        }

        targetTextDisplay.text = result.ToString();
    }

    private void Update()
    {
        if (!hasWon && targetWords != null && targetWords.Count == 0)
        {
            hasWon = true;
            CancelInvoke(nameof(StopGame));
            WinGame();
        }

        if (isTypingStarted && !hasWon)
        {
            timeElapsed += Time.deltaTime;
            UpdateWPMDisplay();
        }

        if (isMovingOffScreen)
        {
            playerAnimator.transform.Translate(Vector3.forward * winMoveSpeed * Time.deltaTime);
        }
    }

    void MoveGameForward()
    {
        playerAnimator.SetBool("isRunning", true);
        backgroundScript.enabled = true;
        groundScript.enabled = true;
    }

    public void StopGame()
    {
        if (hasWon)
            return;

        playerAnimator.SetBool("isRunning", false);
        backgroundScript.enabled = false;
        groundScript.enabled = false;
    }

    void WinGame()
    {
        playerAnimator.SetBool("isRunning", true);
        backgroundScript.enabled = false;
        groundScript.enabled = false;
        isMovingOffScreen = true;
        gameOverCanvas.enabled = true;

    }

    void UpdateWPMDisplay()
    {
        if (wpmDisplay == null || timeElapsed <= 0) return;

        if (timeElapsed < 1.0f) return;

        float minutesElapsed = timeElapsed / 60f;
        
        int currentInputLength = 0;
        if (targetWords != null && targetWords.Count > 0)
        {
            currentInputLength = Mathf.Min(inputField.text.TrimEnd().Length, targetWords.Peek().Length);
        }

        float totalEquivalentWords = (totalTypedCharacters + currentInputLength) / 5f;
        int currentWPM = Mathf.RoundToInt(totalEquivalentWords / minutesElapsed);
        
        wpmDisplay.text = currentWPM.ToString() + " WPM";
    }

    public string GetTargetText()
    {
        return targetText;
    }
}
