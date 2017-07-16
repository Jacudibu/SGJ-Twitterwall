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

    [Header("UI")]
    [SerializeField] private Text hashtagText;
    [SerializeField] private Text updateCounter;
    [SerializeField] private Text tweetCounter;

    [Header("Tweet Parents")]
    [SerializeField] private Transform textTweetParent;
    [SerializeField] private Transform mediaTweetParent;

    [Header("Prefabs")]
    [SerializeField] private GameObject textTweetPrefab;
    [SerializeField] private GameObject mediaTweetPrefab;

    private Queue<Tweet> upcomingTweets = new Queue<Tweet>();
    private List<TweetCard> textTweets = new List<TweetCard>();
    private List<TweetCard> mediaTweets = new List<TweetCard>();

    private int totalTweets;
    
    private void Awake()
    {
        Application.runInBackground = true;

        Clear();
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

        updateCounter.text = timeLeftUntilNextUpdate.ToString("0");
    }

    private void LoadTweets(string hashtag)
    {
        hashtagText.text = hashtag;

        TwitterAPI.instance.FetchAllTweets(hashtag, resultType, SearchTweetsResultsCallBack);
    }

    private void SearchTweetsResultsCallBack(List<Tweet> tweetList)
    {
        tweetList.Sort(Tweet.Compare);

        foreach (Tweet tweet in tweetList)
        {
            upcomingTweets.Enqueue(tweet);
        }
    }

    private IEnumerator Coroutine_HandleTweetSpawnQueue()
    {
        int emptyQueueTimer = 0;
        while (true)
        {
            while (upcomingTweets.Count == 0)
            {
                yield return new WaitForSeconds(2f);
                emptyQueueTimer += 2;
                if (emptyQueueTimer > 300)
                {
                    yield return DisplayOldTweets();
                }
            }

            emptyQueueTimer = 0;
            SpawnTweet(upcomingTweets.Dequeue());

            yield return new WaitForSeconds(Random.Range(2f, 5f));
        }
    }

    private IEnumerator DisplayOldTweets()
    {
        for (int i = 0; i < textTweets.Count || i < mediaTweets.Count; i++)
        {
            if (i < textTweets.Count)
            {
                textTweets[i].transform.SetAsFirstSibling();
                yield return new WaitForSeconds(5f);
            }

            if (i < mediaTweets.Count)
            {
                mediaTweets[i].transform.SetAsFirstSibling();
                yield return new WaitForSeconds(5f);
            }
        }
    }

    private void SpawnTweet(Tweet data)
    {
        if (data.media == null || data.media.Length == 0)
        {
            TweetCard tweet = Instantiate(textTweetPrefab).GetComponent<TweetCard>();
            tweet.Initialize(data);
            tweet.transform.SetParent(textTweetParent);
            tweet.transform.SetAsFirstSibling();
            textTweets.Add(tweet);
        }
        else
        {
            TweetCard tweet = Instantiate(mediaTweetPrefab).GetComponent<TweetCard>();
            tweet.Initialize(data);
            tweet.transform.SetParent(mediaTweetParent);
            tweet.transform.SetAsFirstSibling();
            textTweets.Add(tweet);
        }

        totalTweets++;
        tweetCounter.text = totalTweets + " tweets total!";
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

        totalTweets = 0;
    }

}
