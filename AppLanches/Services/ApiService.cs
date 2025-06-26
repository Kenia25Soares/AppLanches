using AppLanches.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppLanches.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        public  readonly string BaseUrl = "https://z0nkwd9h-7298.uks1.devtunnels.ms/";
        private readonly ILogger<ApiService> _logger;

        JsonSerializerOptions _serializerOptions;

        public ApiService(HttpClient httpClient,
                          ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiResponse<bool>> RegisterUser(string name, string email,
                                                 string phone, string password)
        {
            try
            {
                var register = new Register()
                {
                    Name = name,
                    Email = email,
                    Phone = phone,
                    Password = password
                };

                var json = JsonSerializer.Serialize(register, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Users/Register", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Erro ao enviar requisição HTTP: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Erro ao enviar requisição HTTP: {response.StatusCode}"
                    };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao registrar o usuário: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }


        public async Task<ApiResponse<bool>> Login(string email, string password)
        {
            try
            {
                var login = new Login()
                {
                    Email = email,
                    Password = password
                };

                var json = JsonSerializer.Serialize(login, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Users/Login", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Erro ao enviar requisição HTTP : {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Erro ao enviar requisição HTTP : {response.StatusCode}"
                    };
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);

                Preferences.Set("accessToken", result!.AccessToken);
                Preferences.Set("userId", (int)result.UserId!);
                Preferences.Set("userName", result.UserName);

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro no login : {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> AddItemToCart(ShoppingCart shoppingCart)
        {
            try
            {
                var json = JsonSerializer.Serialize(shoppingCart, _serializerOptions); // Serializa o objeto ShoppingCart para JSON
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/ShoppingCartItems", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Erro ao enviar requisição HTTP: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Erro ao enviar requisição HTTP: {response.StatusCode}"
                    };
                }

                return new ApiResponse<bool> { Data = true }; // Retorna sucesso se a requisição for bem-sucedida
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error ao adicionar item no carinho: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }


        public async Task<ApiResponse<bool>> ConfirmOrder(Order order)
        {
            try
            {
                var json = JsonSerializer.Serialize(order, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Orders", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = response.StatusCode == HttpStatusCode.Unauthorized
                        ? "Unauthorized"
                        : $"Erro ao enviar requisição HTTP: {response.StatusCode}";

                    _logger.LogError($"Erro ao enviar requisição HTTP: {response.StatusCode}");
                    return new ApiResponse<bool> { ErrorMessage = errorMessage };
                }
                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error ao confirmar pedido: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }


        public async Task<ApiResponse<bool>> UploadImageUser(byte[] imageArray)
        {
            try
            {
                var content = new MultipartFormDataContent();
                content.Add(new ByteArrayContent(imageArray), "image", "image.jpg");
                var response = await PostRequest("api/users/uploadimage", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = response.StatusCode == HttpStatusCode.Unauthorized
                      ? "Unauthorized"
                      : $"Erro ao enviar requisição HTTP: {response.StatusCode}";

                    _logger.LogError($"Erro ao enviar requisição HTTP: {response.StatusCode}");
                    return new ApiResponse<bool> { ErrorMessage = errorMessage };
                }
                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao fazer upload da imagem do usuário: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        private async Task<HttpResponseMessage> PostRequest(string uri, HttpContent content)
        {
            var enderecoUrl = BaseUrl + uri;  // Constrói a URL completa para a requisição
            try
            {
                var result = await _httpClient.PostAsync(enderecoUrl, content);  // Envia a requisição POST para o endpoint especificado
                return result;
            }
            catch (Exception ex)
            {
                // Log o erro ou trate conforme necessário
                _logger.LogError($"Erro ao enviar requisição POST para {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }



        public async Task<(bool Data, string? ErrorMessage)> UpdateShoppingCartItemQuantity(int productId, string action)
        {
            try
            {
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var response = await PutRequest($"api/ShoppingCartItems?productId={productId}&action={action}", content); 
                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return (false, errorMessage);
                    }
                    string generalErrorMessage = $"Erro na Requisição: {response.ReasonPhrase}";
                    _logger.LogError(generalErrorMessage);
                    return (false, generalErrorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"Erro de Requisição HTTP: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (false, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Erro inesperado: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (false, errorMessage);
            }
        }


        private async Task<HttpResponseMessage> PutRequest(string uri, HttpContent content)
        {
            var urlAddress = AppConfig.BaseUrl + uri;
            try
            {
                AddAuthorizationHeader();
                var result = await _httpClient.PutAsync(urlAddress, content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao enviar requisiçaõ PUT para {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }


        public async Task<(List<Category>? Categories, string? ErrorMessage)> GetCategories()
        {
            return await GetAsync<List<Category>>("api/categories");
        }


        public async Task<(List<Product>? Products, string? ErrorMessage)> GetProducts(string productType, string categoryId)
        {
            string endpoint = $"api/Products?Search={productType}&categoryId={categoryId}";
            return await GetAsync<List<Product>>(endpoint);
        }


        public async Task<(Product? productDetails, string errorMessage)> GetProductDetails(int productId)
        {
            string endpoint = $"api/products/{productId}";
            return await GetAsync<Product>(endpoint);
        }


        public async Task<(List<ShoppingCartItem>? ShoppingCartItems, string? ErrorMessage)> GetShoppingCartItems(int userId)
        {
            var endpoint = $"api/ShoppingCartItems/{userId}";
            return await GetAsync<List<ShoppingCartItem>>(endpoint);
        }


        public async Task<(ImageProfile? ImageProfile, string? ErrorMessage)> GetUserProfileImage()
        {
            string endpoint = "api/users/userimage";
            return await GetAsync<ImageProfile>(endpoint);
        }


        private async Task<(T? Data, string? ErrorMessage)> GetAsync<T>(string endpoint)
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.GetAsync(AppConfig.BaseUrl + endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync(); //Le a string do conteúdo da resposta
                    var data = JsonSerializer.Deserialize<T>(responseString, _serializerOptions); // Deserialize a resposta JSON para o tipo T
                    return (data ?? Activator.CreateInstance<T>(), null);  // Return default value if data is null
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return (default, errorMessage);
                    }

                    string generalErrorMessage = $"Erro na Requisição: {response.ReasonPhrase}";
                    _logger.LogError(generalErrorMessage);
                    return (default, generalErrorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"Errp de Requisição HTTP: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (JsonException ex)
            {
                string errorMessage = $"Erro de desserialização JSON: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Erro inesperado: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
        }

        private void AddAuthorizationHeader()
        {
            var token = Preferences.Get("accessToken", string.Empty); 
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
