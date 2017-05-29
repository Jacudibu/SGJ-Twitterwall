using UnityEngine;

using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Collections;

namespace Twitter
{
    // Inspired by https://bitbucket.org/yaustar/twitter-search-with-unity
    public class TwitterAPI : MonoBehaviour
    {
        private string oauthNonce = "";
        private string oauthTimeStamp = "";

        public static TwitterAPI instance = null;

        // Use this for initialization
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.LogError("More then one instance of TwitterAPI: " + this.transform.name);
            }
        }

        public void SearchTwitter(string keywords, string resultType, Action<List<Tweet>> callback)
        {
            PrepareOAuthData();
            StartCoroutine(SearchTwitter_Coroutine(keywords, resultType, callback));
        }

        private IEnumerator SearchTwitter_Coroutine(string keywords, string resultType, Action<List<Tweet>> callback)
        {
            // Fix up hashes to be webfriendly
            keywords = Uri.EscapeDataString(keywords);

            string twitterUrl = "https://api.twitter.com/1.1/search/tweets.json";

            SortedDictionary<string, string> twitterParamsDictionary = new SortedDictionary<string, string>
        {
            {"q", keywords},
            {"count", "100"},
            {"result_type", resultType},
        };

            WWW query = CreateTwitterAPIQuery(twitterUrl, twitterParamsDictionary);
            yield return query;

            callback(ParseTwitterJson(query.text));
        }

        private List<Tweet> ParseTwitterJson(string jsonResults)
        {
            Debug.Log(jsonResults);

            JSONObject json = new JSONObject(jsonResults);

            if (json.type == JSONObject.Type.OBJECT)
            {
                for (int i = 0; i < json.list.Count; i++)
                {
                    if (json.keys[i].Equals("statuses"))
                    {
                        return ParseStatuses(json.list[i]);
                    }
                }
            }
            else
            {
                Debug.LogError("Received wrong json format!" + json.Print());
            }

            //      Debug.LogWarning(jsonResults);
            //foreach (IDictionary tweet in tweets)
            //      {
            //	IDictionary userInfo = tweet["user"] as IDictionary;			

            //	TweetSearchTwitterData twitterData = new TweetSearchTwitterData();			
            //	twitterData.tweetText = tweet["text"].ToString();
            //	twitterData.screenName = userInfo["screen_name"].ToString();
            //	twitterData.retweetCount = (long) tweet["retweet_count"];
            //	twitterData.profileImageUrl = userInfo["profile_image_url"].ToString();

            //	twitterDataList.Add(twitterData);
            //} 

            return null;
        }

        private List<Tweet> ParseStatuses(JSONObject statuses)
        {
            List<Tweet> tweets = new List<Tweet>();

            for (int i = 0; i < statuses.list.Count; i++)
            {
                tweets.Add(new Tweet(statuses.list[i]));
            }

            return tweets;
        }

        private WWW CreateTwitterAPIQuery(string twitterUrl, SortedDictionary<string, string> twitterParamsDictionary)
        {
            string signature = CreateSignature(twitterUrl, twitterParamsDictionary);
            Debug.Log("OAuth Signature: " + signature);

            string authHeaderParam = CreateAuthorizationHeaderParameter(signature, this.oauthTimeStamp);
            Debug.Log("Auth Header: " + authHeaderParam);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers["Authorization"] = authHeaderParam;

            string twitterParams = ParamDictionaryToString(twitterParamsDictionary);

            WWW query = new WWW(twitterUrl + "?" + twitterParams, null, headers);
            return query;
        }


        private void PrepareOAuthData()
        {
            oauthNonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture)));
            TimeSpan _timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
            oauthTimeStamp = Convert.ToInt64(_timeSpan.TotalSeconds).ToString(CultureInfo.InvariantCulture);

            // Override the nounce and timestamp here if troubleshooting with Twitter's OAuth Tool
            //oauthNonce = "69db07d069ac50cd673f52ee08678596";
            //oauthTimeStamp = "1442419142";
        }

        // Taken from http://www.i-avington.com/Posts/Post/making-a-twitter-oauth-api-call-using-c
        private string CreateSignature(string url, SortedDictionary<string, string> searchParamsDictionary)
        {
            //string builder will be used to append all the key value pairs
            StringBuilder signatureBaseStringBuilder = new StringBuilder();
            signatureBaseStringBuilder.Append("GET&");
            signatureBaseStringBuilder.Append(Uri.EscapeDataString(url));
            signatureBaseStringBuilder.Append("&");

            //the key value pairs have to be sorted by encoded key
            SortedDictionary<string, string> urlParamsDictionary = new SortedDictionary<string, string>
                             {
                                 {"oauth_version", "1.0"},
                                 {"oauth_consumer_key", APIKeys.authConsumerKey},
                                 {"oauth_nonce", this.oauthNonce},
                                 {"oauth_signature_method", "HMAC-SHA1"},
                                 {"oauth_timestamp", this.oauthTimeStamp},
                                 {"oauth_token", APIKeys.authToken}
                             };

            foreach (KeyValuePair<string, string> keyValuePair in searchParamsDictionary)
            {
                urlParamsDictionary.Add(keyValuePair.Key, keyValuePair.Value);
            }

            signatureBaseStringBuilder.Append(Uri.EscapeDataString(ParamDictionaryToString(urlParamsDictionary)));
            string signatureBaseString = signatureBaseStringBuilder.ToString();

            Debug.Log("Signature Base String: " + signatureBaseString);

            //generation the signature key the hash will use
            string signatureKey =
                Uri.EscapeDataString(APIKeys.authConsumerSecret) + "&" +
                Uri.EscapeDataString(APIKeys.authTokenSecret);

            HMACSHA1 hmacsha1 = new HMACSHA1(
                new ASCIIEncoding().GetBytes(signatureKey));

            //hash the values
            string signatureString = Convert.ToBase64String(
                hmacsha1.ComputeHash(
                    new ASCIIEncoding().GetBytes(signatureBaseString)));

            return signatureString;
        }

        private string CreateAuthorizationHeaderParameter(string signature, string timeStamp)
        {
            string authorizationHeaderParams = String.Empty;
            authorizationHeaderParams += "OAuth ";

            authorizationHeaderParams += "oauth_consumer_key="
                                         + "\"" + Uri.EscapeDataString(APIKeys.authConsumerKey) + "\", ";

            authorizationHeaderParams += "oauth_nonce=" + "\"" +
                                         Uri.EscapeDataString(this.oauthNonce) + "\", ";

            authorizationHeaderParams += "oauth_signature=" + "\""
                                         + Uri.EscapeDataString(signature) + "\", ";

            authorizationHeaderParams += "oauth_signature_method=" + "\"" +
                Uri.EscapeDataString("HMAC-SHA1") +
                "\", ";

            authorizationHeaderParams += "oauth_timestamp=" + "\"" +
                                         Uri.EscapeDataString(timeStamp) + "\", ";

            authorizationHeaderParams += "oauth_token=" + "\"" +
                                         Uri.EscapeDataString(APIKeys.authToken) + "\", ";

            authorizationHeaderParams += "oauth_version=" + "\"" +
                                         Uri.EscapeDataString("1.0") + "\"";
            return authorizationHeaderParams;
        }

        private string ParamDictionaryToString(IDictionary<string, string> paramsDictionary)
        {
            StringBuilder dictionaryStringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> keyValuePair in paramsDictionary)
            {
                //append a = between the key and the value and a & after the value
                dictionaryStringBuilder.Append(string.Format("{0}={1}&", keyValuePair.Key, keyValuePair.Value));
            }

            // Get rid of the extra & at the end of the string
            string paramString = dictionaryStringBuilder.ToString().Substring(0, dictionaryStringBuilder.Length - 1);
            return paramString;
        }
    }
}