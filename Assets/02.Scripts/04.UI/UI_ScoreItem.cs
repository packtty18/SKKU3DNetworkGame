using TMPro;
using UnityEngine;

public class UI_ScoreItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _rankText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _scoreText;

    public void Set(int rank, string nickName, int score)
    {
        _rankText.text = rank.ToString();
        _nameText.text = nickName;
        _scoreText.text = score.ToString();
    }

    public void Clear()
    {
        _rankText.text = "-";
        _nameText.text = "-";
        _scoreText.text = "-";
    }
}
