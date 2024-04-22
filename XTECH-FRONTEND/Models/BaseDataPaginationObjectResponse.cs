namespace XTECH_FRONTEND.Models
{
    public class BaseDataPaginationObjectResponse<T>
    {
        public int status { get; set; }
        public string msg { get; set; }
        public T data { get; set; }
        public int total_item { get; set; }
        public int total_page { get; set; }
    }
}
