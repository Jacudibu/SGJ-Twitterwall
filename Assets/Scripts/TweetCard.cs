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
        content.text = tweet.text.Replace("\\/", "/").Replace("\\n", " ");
        content.text = content.text.Replace("\\u00e4", "ä").Replace("\\u00c4", "Ä");
        content.text = content.text.Replace("\\u00f6", "ö").Replace("\\u00d6", "Ö");
        content.text = content.text.Replace("\\u00fc", "ü").Replace("\\u00dc", "Ü");
        content.text = content.text.Replace("\\u00df", "ß");
        content.text = content.text.Replace("&lt;", "<").Replace("&gt;", ">");
        

        if (tweet.media != null && tweet.media.Length > 0)
        {
            StartCoroutine(Coroutine_LoadImage(tweet.media[0].media_url));
        }
    }

    private IEnumerator Coroutine_LoadUserPicture(string url)
    {
        url = url.Replace("_normal", "_bigger");
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
