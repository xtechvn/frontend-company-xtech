using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XTECH_FRONTEND.Models.News
{
    public class ArticleResponse
    {
        public int id { get; set; }
        public string category_name { get; set; }
        public string title { get; set; }
        public string lead { get; set; }
        public string image_169 { get; set; }
        public string image_43 { get; set; }
        public string image_11 { get; set; }
        public string body { get; set; }
        public string image { get; set; }
        public DateTime publish_date { get; set; }
        public int? position { get; set; }
        public string directlink { get; set; }

    }
}
