using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Twitter;

public class TweetManager : MonoBehaviour
{
    public string hashtag;
    public string resultType = "recent";
    [SerializeField] private TweetGroup mediaTweets;
    [SerializeField] private TweetGroup textTweets;

    [Range(61, 300)]
    public float timeBetweenUpdates = 61f;
    private float timeLeftUntilNextUpdate = 0f;
    private Queue<Tweet> upcomingTweets = new Queue<Tweet>();

    [Header("Prefabs")]
    [SerializeField] private GameObject textTweetPrefab;
    [SerializeField] private GameObject mediaTweetPrefab;

    private TweetUI tweetUI;
    private int totalTweets;

    private void Awake()
    {
        Application.runInBackground = true;
        tweetUI = FindObjectOfType<TweetUI>();
    }

    private void Start()
    {
        LoadTweets(hashtag);
        StartCoroutine(Coroutine_HandleTweetSpawnQueue());
    }

    public void Update()
    {
        timeLeftUntilNextUpdate -= Time.deltaTime;
        if (timeLeftUntilNextUpdate <= 0 || Input.GetKeyDown(KeyCode.Space))
        {
            LoadTweets(hashtag);
            timeLeftUntilNextUpdate = timeBetweenUpdates;
        }

        tweetUI.UpdateTimer(timeLeftUntilNextUpdate);
    }

    private void LoadTweets(string hashtag)
    {
        tweetUI.UpdateHastag(hashtag);

        TwitterAPI.instance.FetchAllTweets(hashtag, resultType, SearchTweetsResultsCallBack);
    }

    private void SearchTweetsResultsCallBack(List<Tweet> tweetList)
    {
        foreach (Tweet tweet in tweetList)
        {
            upcomingTweets.Enqueue(tweet);
        }
    }

    private IEnumerator Coroutine_HandleTweetSpawnQueue()
    {
        while (true)
        {
            while (upcomingTweets.Count == 0)
            {
                yield return new WaitForSeconds(2f);
            }

            SpawnTweet(upcomingTweets.Dequeue());

            yield return new WaitForSeconds(Random.Range(2f, 5f));
        }
    }

    private void SpawnTweet(Tweet data)
    {


        if (data.media == null || data.media.Length == 0)
        {
            TweetCard tweet = Instantiate(textTweetPrefab).GetComponent<TweetCard>();
            tweet.Initialize(data);
            textTweets.AddTweetCard(tweet);
        }
        else
        {
            TweetCard tweet = Instantiate(mediaTweetPrefab).GetComponent<TweetCard>();
            tweet.Initialize(data);
            mediaTweets.AddTweetCard(tweet);
        }

        totalTweets++;
        tweetUI.UpdateTweetCounter(totalTweets);
    }

    public void Clear()
    {
        textTweets.Clear();
        mediaTweets.Clear();

        totalTweets = 0;
    }
}
