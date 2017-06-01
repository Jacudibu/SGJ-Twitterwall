using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using Twitter;

public class TweetCanvas : MonoBehaviour
{
    public string hashtag;
    public string resultType = "recent";
    [Range(61, 300)]
    public float timeBetweenUpdates = 61f;
    private float timeLeftUntilNextUpdate = 0f;

    [SerializeField] private Text hashtagText;
    [SerializeField] private Text updateCounter;
    [SerializeField] private Transform textTweetParent;
    [SerializeField] private Transform mediaTweetParent;
    [SerializeField] private GameObject textTweetPrefab;
    [SerializeField] private GameObject mediaTweetPrefab;

    private List<Tweet> allTweets;
    private List<TweetCard> textTweets = new List<TweetCard>();
    private List<TweetCard> mediaTweets = new List<TweetCard>();

    public void Update()
    {
        timeLeftUntilNextUpdate -= Time.deltaTime;
        if (timeLeftUntilNextUpdate <= 0 || Input.GetKeyDown(KeyCode.Space))
        {
            LoadTweets(hashtag);
            timeLeftUntilNextUpdate = timeBetweenUpdates;
        }

        updateCounter.text = timeLeftUntilNextUpdate.ToString("0");
    }

    private void LoadTweets(string hashtag)
    {
        hashtagText.text = hashtag;

        TwitterAPI.instance.FetchAllTweets(hashtag, resultType, SearchTweetsResultsCallBack);
    }

    private void SearchTweetsResultsCallBack(List<Tweet> tweetList)
    {
        allTweets = tweetList;

        Debug.Log("Tweet Update\n====================================================");
        foreach (Tweet twitterData in tweetList)
        {
            // Debug.Log("Tweet: " + twitterData.ToString());
            SpawnTweet(twitterData);
        }
    }

    private void SpawnTweet(Tweet data)
    {
        if (data.media == null || data.media.Length == 0)
        {
            TweetCard tweet = Instantiate(textTweetPrefab).GetComponent<TweetCard>();
            tweet.Initialize(data);
            tweet.transform.SetParent(textTweetParent);
            textTweets.Add(tweet);
        }
        else
        {
            TweetCard tweet = Instantiate(mediaTweetPrefab).GetComponent<TweetCard>();
            tweet.Initialize(data);
            tweet.transform.SetParent(mediaTweetParent);
            textTweets.Add(tweet);
        }
    }

    public void Clear()
    {
        foreach (TweetCard tweet in textTweets)
        {
            Destroy(tweet.gameObject);
        }

        foreach (TweetCard tweet in mediaTweets)
        {
            Destroy(tweet.gameObject);
        }

        textTweets.Clear();
    }

}
