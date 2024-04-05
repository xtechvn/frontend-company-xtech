using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XTECH_FRONTEND.Models.News.GetDetail
{
    public class GetNewDetailObjectResponse
    {
        public GetNewDetailResponse Details { get; set; }
        public List<ArticleResponse> MostViewedArticles { get; set; }

    }
}
