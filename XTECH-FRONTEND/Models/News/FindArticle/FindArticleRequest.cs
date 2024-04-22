namespace XTECH_FRONTEND.Models.News.FindArticle
{
    public class FindArticleRequest : BasePaginate
    {
        public string title { get; set; }
        public int parent_id { get; set; }

    }
}
