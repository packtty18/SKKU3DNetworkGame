using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class UI_ScoreRank : MonoBehaviour
{
    [ShowInInspector, ReadOnly] private List<UI_ScoreItem> _items;

    private void Start()
    {
        _items = GetComponentsInChildren<UI_ScoreItem>().ToList();
        ScoreManager.OnDataChanged += Refresh;
        Refresh();
    }

    private void OnDestroy()
    {
        ScoreManager.OnDataChanged -= Refresh;
    }

    private void Refresh()
    {
        if (ScoreManager.Instance == null)
        {
            return;
        }

        if (_items == null || _items.Count == 0)
        {
            return;
        }

        var scores = ScoreManager.Instance.Scores;
        if (scores.Count == 0)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].Clear();
            }
            return;
        }

        List<ScoreData> scoreDatas = scores.Values.ToList();
        scoreDatas.Sort((x, y) => y.Score.CompareTo(x.Score));

        int visibleCount = Mathf.Min(_items.Count, scoreDatas.Count);
        for (int i = 0; i < visibleCount; i++)
        {
            ScoreData data = scoreDatas[i];
            _items[i].Set(i + 1, data.Nickname, data.Score);
        }

        for (int i = visibleCount; i < _items.Count; i++)
        {
            _items[i].Clear();
        }
    }
}
