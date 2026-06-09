using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using ArslanTeklifSistemi.Models;
using System.Xml.Linq;
using System.Globalization;
using System.Net.Http;

namespace ArslanTeklifSistemi.Services
{
    public class WooCommerceService
    {
        private readonly HttpClient _httpClient;

        private const string WpBaseUrl = "https://arslanelektromarket.com.tr/wp-json/wc/v3/";
        private const string ConsumerKey = "ck_0000";
        private const string ConsumerSecret = "cs_0000";

        public WooCommerceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<Dictionary<string, decimal>> GetLiveExchangeRatesAsync()
        {
            var rates = new Dictionary<string, decimal> { { "USD", 46.0144m }, { "EUR", 53.0044m } };

            try
            {
                string tcmbUrl = "https://www.tcmb.gov.tr/kurlar/today.xml";

                var request = new HttpRequestMessage(HttpMethod.Get, tcmbUrl);

                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var xmlString = await response.Content.ReadAsStringAsync();
                    XDocument xdoc = XDocument.Parse(xmlString);
                    var currencies = xdoc.Descendants("Currency");

                    foreach (var currency in currencies)
                    {
                        string? currencyCode = currency.Attribute("CurrencyCode")?.Value;

                        if (currencyCode == "USD" || currencyCode == "EUR")
                        {
                            string? forexSellingStr = currency.Element("ForexSelling")?.Value;

                            if (!string.IsNullOrWhiteSpace(forexSellingStr))
                            {
                                if (decimal.TryParse(forexSellingStr.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal rate) && rate > 0)
                                {
                                    rates[currencyCode] = rate;
                                }
                            }
                        }
                    }
                    Console.WriteLine($"TCMB Canlı Kurlar Çekildi -> USD: {rates["USD"]}, EUR: {rates["EUR"]}");
                }
                else
                {
                    Console.WriteLine($"TCMB Sunucusu Hata Döndürdü: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TCMB XML Kur Çekme Hatası: {ex.Message}");
            }

            return rates;
        }
        public async Task<ProductDto?> GetProductBySkuAsync(string sku)
        {
            try
            {
                var requestUrl = $"{WpBaseUrl}products?sku={sku}&consumer_key={ConsumerKey}&consumer_secret={ConsumerSecret}";

                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Sunucu Hata Kodu Döndürdü: {response.StatusCode}");
                    return null;
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"WooCommerce'den Gelen Ham Veri: {jsonString}");

                using var doc = JsonDocument.Parse(jsonString);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                {
                    var productData = root[0];

                    string fxCode = "TL";
                    decimal fxRegular = 0m;
                    decimal fxSale = 0m;
                    string datasheetAttachmentId = "";
                    string realPdfUrl = "";
                    double discountPercent = 0;

                    if (productData.TryGetProperty("meta_data", out var metaData) && metaData.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var meta in metaData.EnumerateArray())
                        {
                            string key = meta.GetProperty("key").GetString() ?? "";
                            var valueElement = meta.GetProperty("value");
                            string valueStr = valueElement.ValueKind != JsonValueKind.Null ? valueElement.ToString() : "";

                            if (key == "_fx_code")
                            {
                                fxCode = string.IsNullOrWhiteSpace(valueStr) ? "TL" : valueStr.ToUpper();
                            }
                            else if (key == "_fx_regular")
                            {
                                decimal.TryParse(valueStr, out fxRegular);
                            }
                            else if (key == "_fx_sale")
                            {
                                decimal.TryParse(valueStr, out fxSale);
                            }
                            else if (key == "urun_belgesi")
                            {
                                datasheetAttachmentId = valueStr.Trim();
                            }
                            else if (key == "_h1c_discount_percent")
                            {
                                double.TryParse(valueStr, out discountPercent);
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(datasheetAttachmentId) && datasheetAttachmentId != "0")
                    {
                        try
                        {
                            var mediaUrl = $"{WpBaseUrl.Replace("wc/v3/", "wp/v2/")}media/{datasheetAttachmentId}?consumer_key={ConsumerKey}&consumer_secret={ConsumerSecret}";

                            var mediaResponse = await _httpClient.GetAsync(mediaUrl);
                            if (mediaResponse.IsSuccessStatusCode)
                            {
                                var mediaJson = await mediaResponse.Content.ReadAsStringAsync();
                                using var mediaDoc = JsonDocument.Parse(mediaJson);

                                if (mediaDoc.RootElement.TryGetProperty("source_url", out var sourceUrlProp))
                                {
                                    realPdfUrl = sourceUrlProp.GetString() ?? "";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"PDF URL Çözme Hatası: {ex.Message}");
                        }
                    }

                    var finalDto = new ProductDto
                    {
                        ProductCode = sku,
                        ProductName = productData.GetProperty("name").GetString() ?? "",
                        UnitPrice = fxRegular > 0m ? fxRegular : 0m,
                        Currency = fxCode,
                        DiscountPercent = discountPercent,
                        DocUrl = realPdfUrl
                    };

                    if (productData.TryGetProperty("images", out var images) && images.ValueKind == JsonValueKind.Array && images.GetArrayLength() > 0)
                    {
                        finalDto.ImageUrl = images[0].GetProperty("src").GetString() ?? "";
                    }

                    return finalDto;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WooCommerce API Hatası: {ex.Message}");
            }

            return null;
        }
    }
}