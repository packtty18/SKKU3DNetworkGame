using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;

public class ScoreManager : PunCallbackSingleton<ScoreManager>
{
    private const string SCORE_KEY = "score";
    private const int LEVEL_THRESHOLD = 1000;
    
    [ShowInInspector, ReadOnly] private int _myScore;
    [ShowInInspector, ReadOnly] private int currentLevel;
    [ShowInInspector, ReadOnly] private Dictionary<int, ScoreData> _scores = new();
    
    public IReadOnlyDictionary<int, ScoreData> Scores => _scores;
    [ShowInInspector, ReadOnly]public int  MyScoreLevel => _myScore / LEVEL_THRESHOLD;
    public int MyScore => _myScore;
    
    public static event Action<int> OnMyScoreChanged;
    public static event Action OnDataChanged;
    
    protected override void OnInitialize()
    {
        _myScore = 0;
    }

    protected override void OnShutdown()
    {
        _myScore = 0;
        currentLevel = 0;
        _scores = null;
        OnMyScoreChanged = null;
        OnDataChanged = null;
    }

    public void InitScore()
    {
        _scores.Clear();
        currentLevel = 0;

        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            Player player = players[i];

            int score = 0;
            TryGetScore(player.CustomProperties, out score);

            _scores[player.ActorNumber] = new ScoreData
            {
                Nickname = player.NickName,
                Score = score
            };
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

    public void Refresh()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.LocalPlayer == null)
        {
            return;
        }

        _scores[PhotonNetwork.LocalPlayer.ActorNumber] = new ScoreData
        {
            Nickname = PhotonNetwork.LocalPlayer.NickName,
            Score = _myScore
        };

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
        OnDataChanged?.Invoke();
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
        if (_scores != null && _scores.TryGetValue(actorNumber, out ScoreData scoreData))
        {
            return scoreData.Score;
        }

        return 0;
    }
}
