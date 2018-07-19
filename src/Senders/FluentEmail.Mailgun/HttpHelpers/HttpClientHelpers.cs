using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FluentEmail.Mailgun.HttpHelpers
{
    public class HttpClientHelpers
    {
        public static HttpContent GetPostBody(Dictionary<string, string> parameters)
        {
            var formatted = parameters.Select(x => x.Key + "=" + x.Value);
            return new StringContent(string.Join("&", formatted), Encoding.UTF8, "application/x-www-form-urlencoded");
        }

        public static HttpContent GetJsonBody(object value)
        {
            return new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");
        }

        public static HttpContent GetMultipartFormDataContentBody(List<KeyValuePair<string, string>> parameters, List<HttpFile> files)
        {
            var mpContent = new MultipartFormDataContent();

            parameters.ForEach(p =>
            {
                mpContent.Add(new StringContent(p.Value), p.Key);
            });

            files.ForEach(file =>
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.Data.CopyTo(memoryStream);
                    mpContent.Add(new ByteArrayContent(memoryStream.ToArray()), file.ParameterName, file.Filename);
                }
            });

            return mpContent;
        }
    }

    public static class HttpClientExtensions
    {
        public static async Task<ApiResponse<T>> Get<T>(this HttpClient client, string url)
        {
            var response = await client.GetAsync(url);
            var qr = await QuickResponse<T>.FromMessage(response);
            return qr.ToApiResponse();
        }

        public static async Task<ApiResponse> GetFile(this HttpClient client, string url)
        {
            var response = await client.GetAsync(url);
            var qr = await QuickFile.FromMessage(response);
            return qr.ToApiResponse();
        }

        public static async Task<ApiResponse<T>> Post<T>(this HttpClient client, string url, Dictionary<string, string> parameters)
        {
            var response = await client.PostAsync(url, HttpClientHelpers.GetPostBody(parameters));
            var qr = await QuickResponse<T>.FromMessage(response);
            return qr.ToApiResponse();
        }

        public static async Task<ApiResponse<T>> Post<T>(this HttpClient client, string url, object data)
        {
            var response = await client.PostAsync(url, HttpClientHelpers.GetJsonBody(data));
            var qr = await QuickResponse<T>.FromMessage(response);
            return qr.ToApiResponse();
        }

        public static async Task<ApiResponse<T>> PostMultipart<T>(this HttpClient client, string url, List<KeyValuePair<string, string>> parameters, List<HttpFile> files)
        {
            var response = await client.PostAsync(url, HttpClientHelpers.GetMultipartFormDataContentBody(parameters, files)).ConfigureAwait(false);
            var qr = await QuickResponse<T>.FromMessage(response);
            return qr.ToApiResponse();
        }

        public static async Task<ApiResponse> Delete(this HttpClient client, string url)
        {
            var response = await client.DeleteAsync(url);
            var qr = await QuickResponse.FromMessage(response);
            return qr.ToApiResponse();
        }
    }

    public class QuickResponse
    {
        public HttpResponseMessage Message { get; set; }

        public string ResponseBody { get; set; }

        public List<ApiError> Errors { get; set; }

        public QuickResponse()
        {
            Errors = new List<ApiError>();
        }

        public ApiResponse ToApiResponse()
        {
            return new ApiResponse()
            {
                Errors = Errors
            };
        }

        public static async Task<QuickResponse> FromMessage(HttpResponseMessage message)
        {
            var response = new QuickResponse();
            response.Message = message;
            response.ResponseBody = await message.Content.ReadAsStringAsync();

            if (!message.IsSuccessStatusCode)
            {
                response.HandleFailedCall();
            }

            return response;
        }

        protected void HandleFailedCall()
        {
            try
            {
                Errors = JsonConvert.DeserializeObject<List<ApiError>>(ResponseBody) ?? new List<ApiError>();

                if (!Errors.Any())
                {
                    Errors.Add(new ApiError()
                    {
                        ErrorMessage = !string.IsNullOrEmpty(ResponseBody) ? ResponseBody : Message.StatusCode.ToString()
                    });
                }
            }
            catch (Exception)
            {
                Errors.Add(new ApiError
                {
                    ErrorMessage = !string.IsNullOrEmpty(ResponseBody) ? ResponseBody : Message.StatusCode.ToString()
                });
            }
        }
    }

    public class QuickResponse<T> : QuickResponse
    {
        public T Data { get; set; }

        public new ApiResponse<T> ToApiResponse()
        {
            return new ApiResponse<T>()
            {
                Errors = Errors,
                Data = Data
            };
        }

        public new static async Task<QuickResponse<T>> FromMessage(HttpResponseMessage message)
        {
            var response = new QuickResponse<T>();
            response.Message = message;
            response.ResponseBody = await message.Content.ReadAsStringAsync();

            if (message.IsSuccessStatusCode)
            {
                try
                {
                    response.Data = JsonConvert.DeserializeObject<T>(response.ResponseBody);
                }
                catch (Exception)
                {
                    response.HandleFailedCall();
                }
            }
            else
            {
                response.HandleFailedCall();
            }

            return response;
        }
    }

    public class QuickFile : QuickResponse<Stream>
    {
        public new static async Task<QuickFile> FromMessage(HttpResponseMessage message)
        {
            var response = new QuickFile();
            response.Message = message;
            response.ResponseBody = await message.Content.ReadAsStringAsync();

            if (message.IsSuccessStatusCode)
            {
                response.Data = await message.Content.ReadAsStreamAsync();
            }
            else
            {
                response.HandleFailedCall();
            }

            return response;
        }
    }
}
