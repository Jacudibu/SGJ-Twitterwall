using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Twitter;

public class TweetCard : MonoBehaviour
{
    public bool ContainsImage
    {
        get { return attachedImage != null; }
    }

    [SerializeField] private Image userPic;
    [SerializeField] private Image attachedImage;
    [SerializeField] private Text handle;
    [SerializeField] private Text content;

    public void Initialize(Tweet tweet)
    {
        StartCoroutine(Coroutine_LoadUserPicture(tweet.user.profile_image_url_https));
        handle.text = "@" + tweet.user.screen_name;
        content.text = tweet.text;

        if (tweet.media != null && tweet.media.Length > 0)
        {
            StartCoroutine(Coroutine_LoadImage(tweet.media[0].media_url));
        }
    }

    private IEnumerator Coroutine_LoadUserPicture(string url)
    {
        WWW www = new WWW(url);
        yield return www;

        Rect rect = new Rect(0, 0, www.texture.width, www.texture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);

        userPic.sprite = Sprite.Create(www.texture, rect, pivot);
    }

    private IEnumerator Coroutine_LoadImage(string url)
    {
        WWW www = new WWW(url);
        yield return www;

        if (www.texture != null)
        {
            Rect rect = new Rect(0, 0, www.texture.width, www.texture.height);
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            attachedImage.sprite = Sprite.Create(www.texture, rect, pivot);
        }
    }
}
