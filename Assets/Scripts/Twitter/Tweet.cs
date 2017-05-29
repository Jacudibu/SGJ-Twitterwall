using System.Collections;
using UnityEngine;
using System;

public class Tweet {
	public string text = "";
	public string screenName = "";
	public string profileImageUrl = "";
	public long retweetCount = 0;
	
    // Creates a tweet from one of the elements of the "statuses"-Array.
    public Tweet(JSONObject json)
    {
        for (int i = 0; i < json.list.Count; i++)
        {
            switch (json.keys[i])
            {
                case "text":
                    text = json.list[i].str;
                    break;
                case "":
                    break;
            }
        }
    }

	public override string ToString()
    {
		return screenName + " posted: \"" + text + "\" and retweeted " + retweetCount.ToString() + " times. Profile image URL: " +  profileImageUrl;
	}
}