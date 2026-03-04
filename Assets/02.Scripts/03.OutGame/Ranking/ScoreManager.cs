using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;

public class ScoreManager : MonoBehaviourPunCallbacks
{
    private const string SCORE_KEY = "score";
    private const int LEVEL_THRESHOLD = 1000;

    public static ScoreManager Instance;

    [ShowInInspector, ReadOnly] private int _myScore;
    public int MyScore => _myScore;
    [ShowInInspector, ReadOnly]private int currentLevel;
    [ShowInInspector, ReadOnly]public int  MyScoreLevel => _myScore / LEVEL_THRESHOLD;

    [ShowInInspector, ReadOnly] private Dictionary<int, ScoreData> _scores = new();
    public IReadOnlyDictionary<int, ScoreData> Scores => _scores;

    public static event Action<int> OnMyScoreChanged;
    public static event Action OnDataChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnJoinedRoom()
    {
        _scores.Clear();
        currentLevel = 0;
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            Player player = players[i];
            if (TryGetScore(player.CustomProperties, out int score))
            {
                _scores[player.ActorNumber] = new ScoreData
                {
                    Nickname = player.NickName,
                    Score = score
                };
            }
        }

        OnDataChanged?.Invoke();
        Refresh();
    }

    public override void OnLeftRoom()
    {
        _myScore = 0;
        _scores.Clear();
        OnDataChanged?.Invoke();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (_scores.Remove(otherPlayer.ActorNumber))
        {
            OnDataChanged?.Invoke();
        }
    }
    [Button]
    public void AddScore(int score)
    {
        _myScore += score;
        Refresh();
    }
    [Button]
    public void SubtractScore(int score)
    {
        _myScore = Mathf.Max(_myScore - score, 0);
        Refresh();
    }

    private void Refresh()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.LocalPlayer == null)
        {
            return;
        }

        Hashtable hash = new Hashtable
        {
            [SCORE_KEY] = _myScore
        };
        
        if (MyScoreLevel != currentLevel)
        {
            currentLevel = MyScoreLevel;
            OnMyScoreChanged?.Invoke(currentLevel);
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (Instance == null)
        {
            return;
        }

        if (!TryGetScore(changedProps, out int score))
        {
            return;
        }

        ScoreData scoreData = new ScoreData
        {
            Nickname = targetPlayer.NickName,
            Score = score
        };

        _scores[targetPlayer.ActorNumber] = scoreData;
        OnDataChanged?.Invoke();
        Debug.Log($"Player {scoreData.Nickname} score: {scoreData.Score}");
    }

    private bool TryGetScore(Hashtable props, out int score)
    {
        score = 0;

        if (props == null || !props.ContainsKey(SCORE_KEY))
        {
            return false;
        }

        score = (int)props[SCORE_KEY];
        return true;
    }

    public int GetLevelByActorNumber(int actorNumber)
    {
        return GetScoreByActorNumber(actorNumber) / LEVEL_THRESHOLD;
    }
    private int GetScoreByActorNumber(int actorNumber)
    {
        return Scores[actorNumber].Score;
    }
}
