using UnityEngine;
using TMPro;

public class TypeManager : MonoBehaviour
{
    public TMP_Text targetTextDisplay;
    public TMP_InputField inputField;

    public Animator playerAnimator;
    public MoveLeft backgroundScript;
    public MoveLeft groundScript;

    private string wordToType = "Tôi dìu Tường dọc bờ rào, đầu loay hoay nghĩ cách chuộc lỗi với nó. Tôi tự hứa với mình: mai mốt nếu thằng Tường gặp phải hoạn nạn gì, bị ba tôi phạt đánh đòn vì tội ham chơi bỏ bê bài vở chẳng hạn, tôi sẽ xung phong nhận tội thay nó, tôi sẽ nói với ba tôi là chính tôi xúi thằng Tường đi chơi… Một giọng nói sang sảng thình lình vang lên cắt ngang những ý nghĩ tốt đẹp trong đầu tôi: - Hai đứa bay không ngủ trưa trốn ra đây làm gì đó?";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetTextDisplay.text = wordToType;
        inputField.ActivateInputField();
        backgroundScript.enabled = false;
        groundScript.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    void CheckInput()
    {
        if (inputField.text == wordToType)
        {
            MoveGameForward();
        }
        else{
            StopGame();
        }
    }

    void MoveGameForward()
    {
        playerAnimator.SetBool("isRunning", true);
        backgroundScript.enabled = true;
        groundScript.enabled = true;
    }

    void StopGame()
    {
        playerAnimator.SetBool("isRunning", false);
        backgroundScript.enabled = false;
        groundScript.enabled = false;
    }
}
