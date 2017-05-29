using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TweetCanvas : MonoBehaviour
{
    public string hashtag;
    public string resultType = "recent";
    public float timeBetweenUpdates = 120f;

    [SerializeField] private Text hashtagText;
    [SerializeField] private Transform textTweetParent;
    [SerializeField] private Transform imageTweetParent;
    [SerializeField] private GameObject textTweetPrefab;
    [SerializeField] private GameObject imageTweetPrefab;

    private List<TweetDisplay> tweets = new List<TweetDisplay>();

    public void Start()
    {
        StartCoroutine(Coroutine_UpdateTweets());
    }

    private IEnumerator Coroutine_UpdateTweets()
    {   
        while (true)
        {
            LoadTweets();
            yield return new WaitForSecondsRealtime(timeBetweenUpdates);
        }
    }

    private void LoadTweets()
    {
        LoadTweets(hashtag);
    }

    private void LoadTweets(string hashtag)
    {
        hashtagText.text = hashtag;

        Clear();
        TwitterAPI.instance.SearchTwitter(hashtag, resultType, SearchTweetsResultsCallBack);
    }

    private void SearchTweetsResultsCallBack(List<TweetSearchTwitterData> tweetList)
    {
        Debug.Log("Tweet Update\n====================================================");
        foreach (TweetSearchTwitterData twitterData in tweetList)
        {
            Debug.Log("Tweet: " + twitterData.ToString());
            SpawnTweet(twitterData);
        }
    }

    private void SpawnTweet(TweetSearchTwitterData data)
    {
        TweetDisplay tweet = Instantiate(textTweetPrefab).GetComponent<TweetDisplay>();
        tweet.Initialize(data);
        tweet.transform.SetParent(textTweetParent);

        tweets.Add(tweet);
    }

    public void Clear()
    {
        foreach (TweetDisplay tweet in tweets)
        {
            Destroy(tweet.gameObject);
        }

        tweets.Clear();
    }

}
