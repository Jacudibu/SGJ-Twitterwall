using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Twitter
{
    // Basic Class representing a twitter user. Stuff that's missing is just commented out in its declaration.
    // Fields share their name within the twitter API so that they can be filled in by Unity's JsonUtility.
    // For more information about each field, have a look at https://dev.twitter.com/overview/api/users
    // Note: Stuff that's already flagged as deprecated in the docs doesn't show up here.
    public class User
    {
        //public bool contributors_enabled;
        //public date created_at;
        //public bool default_profile;
        //public bool default_profile_image;
        public string description;
        //public Entity entities
        public int favourites_count;
        //public bool? follow_request_sent;
        //public int followers_count;
        //public int friends_count;
        //public bool geo_enabled;
        public long id;
        public string id_str;
        //public string lang
        //public int listed_count;
        //public string location;
        public string name;
        //public string profile_background_color;
        //public string profile_background_image_url_https;
        //public bool profile_background_tile;
        //public string profile_banner_url;
        public string profile_image_url_https;
        //public string profile_link_color;
        //public string profile_sidebar_border_color;
        //public string profile_sidebar_fill_color;
        //public string profile_text_color;
        //public bool profile_use_background_image
        //public bool protected;
        public string screen_name;
        //public List<Tweet> status;
        //public int statuses_count;
        //public string time_zone;
        //public string url;
        //public int utc_offset;
        //public bool verified;
        //public string withheld_in_countries;
        //public string withheld_scope;
    }
}