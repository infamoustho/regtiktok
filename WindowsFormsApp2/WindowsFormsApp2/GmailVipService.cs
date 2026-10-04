using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WindowsFormsApp2
{
    public class GmailVipConfig
    {
        public string ApiKey { get; set; } = "";
        public int ProductId { get; set; } = 1;
        public int Amount { get; set; } = 5;
        public bool AutoBuy { get; set; } = true;
    }

    public class GmailVipProfileResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public string Username { get; set; } = "";
        public decimal Money { get; set; } = 0;
    }

    public class GmailVipProductItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public string Stock { get; set; } = "";
        public string CategoryName { get; set; } = "";

        public override string ToString() => $"[{Id}] {Name} - {Price:N0}đ";
    }

    public class GmailVipBuyResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public string TransId { get; set; } = "";
        public List<string> Data { get; set; } = new List<string>();
    }

    public class GmailVipService
    {
        private static readonly HttpClient httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
        private const string BaseUrl = "https://gmailvip.vn";
        private const string ConfigFileName = "gmailvip_config.json";

        public static GmailVipConfig LoadConfig()
        {
            try
            {
                if (File.Exists(ConfigFileName))
                {
                    string json = File.ReadAllText(ConfigFileName, Encoding.UTF8);
                    return JsonConvert.DeserializeObject<GmailVipConfig>(json) ?? new GmailVipConfig();
                }
            }
            catch { }
            return new GmailVipConfig();
        }

        public static void SaveConfig(GmailVipConfig config)
        {
            try
            {
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(ConfigFileName, json, Encoding.UTF8);
            }
            catch { }
        }

        /// <summary>
        /// Lấy thông tin tài khoản và số dư từ gmailvip.vn
        /// </summary>
        public static async Task<GmailVipProfileResult> GetProfileAsync(string apiKey)
        {
            var res = new GmailVipProfileResult();
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                res.Message = "Chưa nhập API Key";
                return res;
            }

            try
            {
                string url = $"{BaseUrl}/api/profile.php?api_key={Uri.EscapeDataString(apiKey.Trim())}";
                var response = await httpClient.GetAsync(url);
                string jsonStr = await response.Content.ReadAsStringAsync();

                var jo = JObject.Parse(jsonStr);
                string status = jo["status"]?.ToString();
                res.Message = jo["msg"]?.ToString() ?? "";

                if (status == "success")
                {
                    res.Success = true;
                    var dataObj = jo["data"];
                    if (dataObj != null)
                    {
                        res.Username = dataObj["username"]?.ToString() ?? "";
                        if (decimal.TryParse(dataObj["money"]?.ToString(), out decimal m))
                        {
                            res.Money = m;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = "Lỗi kết nối API: " + ex.Message;
            }

            return res;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm từ gmailvip.vn
        /// </summary>
        public static async Task<List<GmailVipProductItem>> GetProductsAsync(string apiKey)
        {
            var list = new List<GmailVipProductItem>();
            if (string.IsNullOrWhiteSpace(apiKey)) return list;

            try
            {
                string url = $"{BaseUrl}/api/products.php?api_key={Uri.EscapeDataString(apiKey.Trim())}";
                var response = await httpClient.GetAsync(url);
                string jsonStr = await response.Content.ReadAsStringAsync();

                var jo = JObject.Parse(jsonStr);
                if (jo["status"]?.ToString() == "success" && jo["categories"] is JArray cats)
                {
                    foreach (var cat in cats)
                    {
                        string catName = cat["name"]?.ToString() ?? "";
                        if (cat["products"] is JArray prods)
                        {
                            foreach (var p in prods)
                            {
                                int.TryParse(p["id"]?.ToString(), out int id);
                                string name = p["name"]?.ToString() ?? "";
                                decimal.TryParse(p["price"]?.ToString(), out decimal price);
                                string stock = p["stock"]?.ToString() ?? "";

                                if (id > 0)
                                {
                                    list.Add(new GmailVipProductItem
                                    {
                                        Id = id,
                                        Name = name,
                                        Price = price,
                                        Stock = stock,
                                        CategoryName = catName
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            return list;
        }

        /// <summary>
        /// Mua tài khoản email tự động qua API gmailvip.vn
        /// </summary>
        public static async Task<GmailVipBuyResult> BuyEmailAsync(string apiKey, int productId, int amount, string coupon = "")
        {
            var result = new GmailVipBuyResult();
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                result.Message = "Thiếu API Key GmailVIP";
                return result;
            }

            if (productId <= 0)
            {
                result.Message = "ID sản phẩm không hợp lệ";
                return result;
            }

            if (amount <= 0) amount = 1;

            try
            {
                string url = $"{BaseUrl}/api/buy_product";
                var formValues = new Dictionary<string, string>
                {
                    { "action", "buyProduct" },
                    { "id", productId.ToString() },
                    { "amount", amount.ToString() },
                    { "coupon", coupon ?? "" },
                    { "api_key", apiKey.Trim() }
                };

                using (var content = new FormUrlEncodedContent(formValues))
                {
                    var response = await httpClient.PostAsync(url, content);
                    string jsonStr = await response.Content.ReadAsStringAsync();

                    var jo = JObject.Parse(jsonStr);
                    string status = jo["status"]?.ToString();
                    result.Message = jo["msg"]?.ToString() ?? "";
                    result.TransId = jo["trans_id"]?.ToString() ?? "";

                    if (status == "success")
                    {
                        result.Success = true;
                        if (jo["data"] is JArray arr)
                        {
                            foreach (var item in arr)
                            {
                                string line = item?.ToString()?.Trim();
                                if (!string.IsNullOrEmpty(line))
                                {
                                    result.Data.Add(line);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = "Lỗi khi gọi API mua hàng: " + ex.Message;
            }

            return result;
        }
    }
}
