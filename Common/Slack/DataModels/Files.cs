namespace Common.Slack.DataModels
{
    public class File
    {
        public string id { get; set; }
        public int created { get; set; }
        public int timestamp { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string mimetype { get; set; }
        public string filetype { get; set; }
        public string pretty_type { get; set; }
        public string user { get; set; }
        public bool editable { get; set; }
        public int size { get; set; }
        public string mode { get; set; }
        public bool is_external { get; set; }
        public string external_type { get; set; }
        public bool is_public { get; set; }
        public bool public_url_shared { get; set; }
        public bool display_as_bot { get; set; }
        public string username { get; set; }
        public string url_private { get; set; }
        public string url_private_download { get; set; }
        public string thumb_64 { get; set; }
        public string thumb_80 { get; set; }
        public string thumb_360 { get; set; }
        public int thumb_360_w { get; set; }
        public int thumb_360_h { get; set; }
        public string thumb_480 { get; set; }
        public int thumb_480_w { get; set; }
        public int thumb_480_h { get; set; }
        public string thumb_160 { get; set; }
        public string thumb_720 { get; set; }
        public int thumb_720_w { get; set; }
        public int thumb_720_h { get; set; }
        public string thumb_800 { get; set; }
        public int thumb_800_w { get; set; }
        public int thumb_800_h { get; set; }
        public string thumb_960 { get; set; }
        public int thumb_960_w { get; set; }
        public int thumb_960_h { get; set; }
        public string thumb_1024 { get; set; }
        public int thumb_1024_w { get; set; }
        public int thumb_1024_h { get; set; }
        public int image_exif_rotation { get; set; }
        public int original_w { get; set; }
        public int original_h { get; set; }
        public string thumb_tiny { get; set; }
        public string permalink { get; set; }
        public string permalink_public { get; set; }
        public bool is_starred { get; set; }
        public bool has_rich_preview { get; set; }
        public string messageTs { get; set; }
    }
}