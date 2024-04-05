namespace XTECH_FRONTEND.Infrastructure.Utilities.Constants
{
    public class SystemConstants
    {
        public const int AdavigoSuccessStatus = 0;
        public const int CacheDay = 1;
     

        public class AdavigoApiRoutes
        {
            
            public const string GetNewsCategory = "/api/news/get-category.json";
            public const string GetNewsByCategoryId = "/api/news/get-list-by-categoryid-order.json";
       
            public const string GetNewsDetail = "/api/news/get-detail.json";
            public const string FindArticle = "/api/news/find-article.json";
            public const string GetNewsByTag = "/api/news/get-list-by-tag-order.json";

            public const string Domain_API = "http://be.x-tech.vn";
            public const string api_manual = "AAAAB3NzaC1yc2EAAAADAQABAAABAQC+6zVy2tuIFTDWo97E52chdG1QgzTnqEx8tItL+m5x39BzrWMv5RbZZJbB0qU3SMeUgyynrgBdqSsjGk6euV3+97F0dYT62cDP2oBCIKsETmpY3UUs2iNNxDVvpKzPDE4VV4oZXwwr1kxurCiy+8YC2Z0oYdNDlJxd7+80h87ecdYS3olv5huzIDaqxWeEyCvGDCopiMhr+eh8ikwUdTOEYmgQwQcWPCeYcDDZD8afgBMnB6ys2i51BbLAap16R/B83fB78y0N04qXs3rg4tWGhcVhVyWL1q5PmmweesledOWOVFowfO6QIwDSvBwz0n3TstjXWF4JPbdcAQ8VszUj";


        }

    

        public class VinwonderTypeCode
        {
            public const string TE = "TE";
            public const string NL = "NL";
            public const string NCT = "NCT";
        }

        public class ArticleTypes
        {
            public const int Article = 0;
            public const int Video = 1;
        }
    }
}
