using System.Collections;
using UnityEngine;
using System;

using Twitter.Entities;

namespace Twitter
{
    public class Tweet
    {
        public static long newestID = 0;

        public string text;
        public long id;
        public long retweetCount;
        public User user;

        public Media[] media;


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
                    case "id":
                        id = json.list[i].i;
                        if (id > newestID)
                            newestID = id;

                        break;
                    case "user":
                        user = JsonUtility.FromJson<User>(json.list[i].ToString());
                        break;
                    case "entities":
                        ParseEntities(json.list[i]);
                        break;
                }
            }
        }

        public void ParseEntities(JSONObject entities)
        {
            for (int i = 0; i < entities.list.Count; i++)
            {
                JSONObject obj = entities.list[i];

                switch (entities.keys[i])
                {
                    case "media":
                        media = new Media[obj.list.Count];
                        for (int current = 0; current < obj.list.Count; current++)
                        {
                            media[current] = JsonUtility.FromJson<Media>(obj.list[current].ToString());
                        }
                        break;

                    case "urls":
                        break;

                    case "user_mentions":
                        break;

                    case "hashtags":
                        break;

                    case "symbols":
                        break;

                    case "extended_entities":
                        break;
                }
            }
        }

        public override string ToString()
        {
            return user.screen_name + " posted: \"" + text + "\" and retweeted " + retweetCount.ToString() + " times. Profile image URL: " + user.profile_image_url_https;
        }
    }
}