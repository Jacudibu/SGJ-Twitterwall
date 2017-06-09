using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweetGroup : MonoBehaviour
{
    public List<TweetCard> cards;

    [Header("Auto Positioning")]
    public int columns = 1;
    public Vector2 tweetOffset;

    private void Start()
    {
        Clear();
    }

    public void AddTweetCard(TweetCard card)
    {
        cards.Insert(0, card);

        card.transform.SetParent(this.transform);
        card.transform.SetAsFirstSibling();

        UpdateTweetPositions();
    }

    private void UpdateTweetPositions()
    {
        Vector2 targetPos = new Vector2();
        for (int i = 0; i < cards.Count; i++)
        {
            targetPos.x = tweetOffset.x * (i % columns);
            targetPos.y = tweetOffset.y * (i);

            StartCoroutine(Coroutine_AnimateTweetMovement(cards[i].transform, targetPos));
        }
    }

    private IEnumerator Coroutine_AnimateTweetMovement(Transform cardTransform, Vector2 targetPosition)
    {
        cardTransform.localPosition = targetPosition;
        yield return null;
    }

    public void Clear()
    {
        foreach (TweetCard card in GetComponentsInChildren<TweetCard>())
        {
            Destroy(card.gameObject);
        }
    }
}
