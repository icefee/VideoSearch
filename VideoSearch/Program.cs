using System.Diagnostics;
using System.Net.Http.Json;

namespace VideoSearch
{

    class Api
    {
        public static string url = "https://code-in-life.netlify.app";
    }

    class Seach
    {
        public static HttpClient httpClient = new();

        public static string GetUserInput(string message) {

            Console.WriteLine(message);
            string? wd = Console.ReadLine();

            if (wd != null && wd != "") {
                return wd;
            }
            else
            {
                Console.WriteLine("输入不合法");
                return GetUserInput(message);
            }
        }

        public static async Task StartSearch(string wd) {
            Console.WriteLine("正在搜索...");
            SearchResult[]? searchResult = await GetSearchSearch(wd);
            if (searchResult != null)
            {
                Console.WriteLine("请求成功, 获取到" + searchResult.Length.ToString() + "条数据");
                DisplaySearchResult(searchResult);
                PickVideo();
            }
            else
            {
                Console.WriteLine("请求数据失败, 请检查网络设置");
            }
        }

        public static async Task<SearchResult[]?> GetSearchSearch(string wd) {
            string url = Api.url + "/api/video/list?s=" + System.Web.HttpUtility.UrlEncode(wd);
            SearchResult[]? searchResult = null;
            HttpResponseMessage response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                ApiResult? apiResult = await response.Content.ReadFromJsonAsync<ApiResult>();
                if (apiResult?.code == 0)
                {
                    searchResult = apiResult?.data;
                }
            }
            return searchResult;
        }

        public static void DisplaySearchResult(SearchResult[] result) {
            
            foreach (SearchResult resultItem in result.Reverse())
            {
                Console.WriteLine($"\n [ {resultItem.name}  Star {resultItem.rating} ]");

                foreach (SearchResultData dataItem in resultItem.data!)
                {
                    Console.WriteLine("\n-------------------------------------");
                    Console.WriteLine($"[编号] {resultItem.key}_{dataItem.id}");
                    Console.WriteLine($"[名称] {dataItem.name} {dataItem.note}");
                    Console.WriteLine($"[分类] {dataItem.type}");
                    Console.WriteLine($"[更新] {dataItem.last}");
                    Console.WriteLine("-------------------------------------\n");
                }
                Console.WriteLine("\n ********************************* \n");
            }
        }

        public static void PickVideo() {
            string id = GetUserInput("请输入要查看的编号, 输入x重新搜索");
            if (id == "x")
            {
                StartSearch();
                return;
            }
            List<string> args = id.Split('_').ToList();
            if (args.Count == 2) {
                Process.Start(new ProcessStartInfo($"https://c.stormkit.dev/video?api={args[0]}&id={args[1]}") { UseShellExecute = true });
            }
            else
            {
                Console.WriteLine("非法的编号, 请重新输入");
            }
            PickVideo();
        }

        public static void StartSearch()
        {
            string keyword = GetUserInput("请输入要搜索的关键词");
            StartSearch(keyword).GetAwaiter().GetResult();
        }

        public static void Main()
        {
            StartSearch();
        }
    }

    class ApiResult
    {
        public int code { get; set; }
        public SearchResult[]? data { get; set; }
        public string? msg { get; set; }
    }

    class SearchResult
    {
        public string? key { get; set; }
        public string? name { get; set; }
        public double? rating { get; set; }
        public SearchResultPage? page { get; set; }
        public SearchResultData[]? data { get; set; }
    }

    class SearchResultPage
    {
        public int? page { get; set; }
        public int? pagecount { get; set; }
        public int? pagesize { get; set; }
        public int? recordcount { get; set; }
    }

    class SearchResultData
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public int? tid { get; set; }
        public string? type { get; set; }
        public string? note { get; set; }
        public string? dt { get; set; }
        public string? last { get; set; }
    }
}
