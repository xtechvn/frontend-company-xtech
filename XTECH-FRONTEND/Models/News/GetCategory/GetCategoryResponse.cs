using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XTECH_FRONTEND.Models.News.GetCategory
{
    public class GetCategoryObjResponse : BaseResponse
    {
        public List<GetCategoryResponse> categories { get; set; }
        public GetCategoryObjResponse()
        {
            categories = new List<GetCategoryResponse>();
        }
    }
}
