using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XTECH_FRONTEND.Models.News.GetListByCategoryId
{
    public class GetListByCategoryIdResponse : BaseResponse
    {
        public List<ArticleResponse> data { get; set; }
        public List<ArticleResponse> pinned  { get; set; }
        public int total_item { get; set; }
        public int total_page { get; set; }
        public GetListByCategoryIdResponse()
        {
            data = new List<ArticleResponse>();
            pinned = new List<ArticleResponse>();
        }
    }
}
