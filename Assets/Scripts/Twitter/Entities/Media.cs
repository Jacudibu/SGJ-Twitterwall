using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Twitter.Entities {

    // Class to store Information regarding Media Entities.
    // https://dev.twitter.com/overview/api/entities-in-twitter-objects#media
    public struct Media
    {
        public int id;
        public string id_str;
        public string media_url;
        public string media_url_https;
        public string url;
        public string display_url;
        // public ?? sizes
        public string type;
        public int[] indices;
    }
}