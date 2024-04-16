using XTECH_FRONTEND.Models.News;

namespace XTECH_FRONTEND.Models
{
    public class GetListByCategoryIdResponse: BaseObjectResponse
    {
        public List<ArticleResponse> data { get; set; }
        public List<ArticleResponse> pinned { get; set; }
        public int total_item { get; set; }
        public int total_page { get; set; }
        public GetListByCategoryIdResponse()
        {
            data = new List<ArticleResponse>();
            pinned = new List<ArticleResponse>();
        }
    }
}
