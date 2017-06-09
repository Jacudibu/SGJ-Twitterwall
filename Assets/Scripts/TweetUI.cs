using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using Twitter;

public class TweetUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Text hashtagText;
    [SerializeField] private Text updateCounter;
    [SerializeField] private Text tweetCounter;

    public void UpdateTweetCounter(int count)
    {
        tweetCounter.text = count + " Tweets total!";
    }

    public void UpdateTimer(float value)
    {
        updateCounter.text = value.ToString("0");
    }

    public void UpdateHastag(string hashtag)
    {
        hashtagText.text = "#" + hashtag;
    }
}
