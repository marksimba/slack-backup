using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using Common.Slack.ResponseModels;

using SlackDataModels = Common.Slack.DataModels;

namespace Common.Slack
{
    public class SlackActions
    {
        private string _channel;
        private string _token;
        private readonly Logger _logger;
        private string _baseUri = "https://slack.com/api";

        public SlackActions()
        {
            _channel = Environment.GetEnvironmentVariable("CHANNEL");
            _token = Environment.GetEnvironmentVariable("SLACKTOKEN");
            _logger = new Logger();
        }

        //Constructor mainly for testing
        public SlackActions(string token)
        {
            _token = token;
            _logger = new Logger();
        }

        /// <summary>
        /// A Function that send a message to a Slack
        /// </summary>
        /// <param name="message"> Message to send</param>
        /// <param name="channel"> Slack channel tos end message to</param>
        /// <returns>HttpStatusCode</returns>

        public async Task<HttpStatusCode> Message(string message, string channel = null)
        {

            string requestType = "POST";
            string uri = $"{_baseUri}/chat.postMessage";

            using (var httpClient = new HttpClient())
            {
                using(var request = new HttpRequestMessage(new HttpMethod(requestType), uri))
                {
                    //request.Content = new StringContent(JsonConvert.SerializeObject(slackRequest), Encoding.UTF8, "application/x-www-form-urlencoded");
                    request.Content = new SlackRequest(_token, channel != null ? channel : _channel, message).getFormData();
                    var response = await httpClient.SendAsync(request);
                    return response.StatusCode;
                }
            }
        }
        /// <summary>
        /// A Function that returns all slack users
        /// </summary>
        /// <returns>All Slack Users</returns>
        public async Task<List<SlackDataModels :: User>> GetAllUsers()
        {
            string requestType = "GET";
            string uri = $"{_baseUri}/users.list?token={_token}";

            using (var httpClient = new HttpClient())
            {
                using(var request = new HttpRequestMessage(new HttpMethod(requestType), uri))
                {
                    var response = await httpClient.SendAsync(request);
                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    if(response.StatusCode == HttpStatusCode.OK)
                    {
                        Users users =  JsonConvert.DeserializeObject<Users>(responseBody);
                        if(users.ok)
                        {
                            return users.members;
                        }
                        else
                        {
                            throw new Exception($"Unable to get users. Response: {users.error}");
                        }
                    }    
                    else
                    {
                        throw new Exception($"Unable to get users. Response: {responseBody}");
                    }           
                }
            }
        }
        /// <summary>
        /// A Function that gets a specific Slack User
        /// </summary>
        /// <param name="userId"> Id of user to get </param>
        /// <returns>User</returns>

        public async Task<User> GetUser(string userId)
        {
            string requestType = "GET";
            string uri = $"{_baseUri}/users.info?token={_token}&user={userId}";

            using (var httpClient = new HttpClient())
            {
                using(var request = new HttpRequestMessage(new HttpMethod(requestType), uri))
                {
                    var response = await httpClient.SendAsync(request);
                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    if(response.StatusCode == HttpStatusCode.OK)
                    {
                        User user =  JsonConvert.DeserializeObject<User>(responseBody);
                        if(user.ok)
                        {
                            return user;
                        }
                        else
                        {
                            throw new Exception($"Unable to get user. Response: {user.error}");
                        }
                    }    
                    else
                    {
                        throw new Exception($"Unable to get user. Response: {responseBody}");
                    }           
                }
            }
        }

        /// <summary>
        /// A Function that gets all Slack channels
        /// </summary>
        /// <returns>All Slack Channels</returns>
        public async Task<List<SlackDataModels :: Channel>> GetAllChannels()
        {
            string requestType = "GET";
            string uri = $"{_baseUri}/conversations.list?token={_token}";

            using (var httpClient = new HttpClient())
            {
                using(var request = new HttpRequestMessage(new HttpMethod(requestType), uri))
                {
                    var response = await httpClient.SendAsync(request);
                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    if(response.StatusCode == HttpStatusCode.OK)
                    {
                        Channels channels =  JsonConvert.DeserializeObject<Channels>(responseBody);
                        if(channels.ok)
                        {
                            return channels.channels;
                        }
                        else
                        {
                            throw new Exception($"Unable to get channels. Response: {channels.error}");
                        }
                    }    
                    else
                    {
                        throw new Exception($"Unable to get channel. Response: {responseBody}");
                    }           
                }
            }
        }

        /// <summary>
        /// A Function that gets Slack Messages from a channel
        /// </summary>
        /// <param name="channelId"> Channel to get messages from </param>
        /// <returns>Messages</returns>
        public List<SlackDataModels :: Message> GetAllMessages(string channelId)
        {
            bool hasMore = true;
            string latest = null;

            List<SlackDataModels :: Message> messageList = new List<SlackDataModels :: Message>();
            while(hasMore)
            {
                Messages messages = GetChannelHistory(channelId, latest).Result;
                //Combines lists
                messageList = messageList.Concat(messages.messages).ToList();
                if(messages.messages.Count > 0)
                {
                    latest = messages.messages[messages.messages.Count-1].ts; //Gets the timestamp from the last item.
                }
                hasMore = messages.has_more;
            }
            return messageList;
        }

        /// <summary>
        /// A Function that gets a specific Slack Channel
        /// </summary>
        /// <param name="channelId"> Id of channel to get </param>
        /// <returns>Channel</returns>
        public async Task<Channel> GetChannel(string channelId)
        {
            string requestType = "GET";
            string uri = $"{_baseUri}/conversations.info?token={_token}&channel={channelId}";

            using (var httpClient = new HttpClient())
            {
                using(var request = new HttpRequestMessage(new HttpMethod(requestType), uri))
                {
                    var response = await httpClient.SendAsync(request);
                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    if(response.StatusCode == HttpStatusCode.OK)
                    {
                        Channel channel =  JsonConvert.DeserializeObject<Channel>(responseBody);
                        if(channel.ok)
                        {
                            return channel;
                        }
                        else
                        {
                            throw new Exception($"Unable to get channel. Response: {channel.error}");
                        }
                    }    
                    else
                    {
                        throw new Exception($"Unable to get channel. Response: {responseBody}");
                    }           
                }
            }
        }

        /// <summary>
        /// A Function that continually processes GetAllMessages
        /// until all messages in a channel are returned
        /// </summary>
        /// <param name="channel"> Id of channel to get </param>
        /// <param name="latest"> Latest message received </param>
        /// <param name="limit"> How many messages to return </param>
        /// <param name="inclusive"> Include latest in messages? </param>
        /// <returns>Messages</returns>
        public async Task<Messages> GetChannelHistory(string channel, string latest, int limit, bool inclusive)
        {
            string requestType = "GET";
            string uri = $"{_baseUri}/channels.history?token={_token}&channel={channel}&latest={latest}&limit={limit}&inclusive={inclusive}";
            Console.WriteLine(uri);

            using (var httpClient = new HttpClient())
            {
                using(var request = new HttpRequestMessage(new HttpMethod(requestType), uri))
                {
                    var response = await httpClient.SendAsync(request);
                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    if(response.StatusCode == HttpStatusCode.OK)
                    {
                        Messages messages =  JsonConvert.DeserializeObject<Messages>(responseBody);
                        if(messages.ok)
                        {
                            return messages;
                        }
                        else
                        {
                            throw new Exception($"Unable to get messages. Response: {messages.error}");
                        }
                    }    
                    else
                    {
                        throw new Exception($"Unable to get messages. Response: {responseBody}");
                    }           
                }
            }
        }

        /// <summary>
        /// A Function that continually processes GetAllMessages
        /// until all messages in a channel are returned
        /// </summary>
        /// <param name="channel"> Id of channel to get </param>
        /// <param name="latest"> Latest message received </param>
        /// <returns>Messages</returns>

        public async Task<Messages> GetChannelHistory(string channel, string latest = "now")
        {
            string requestType = "GET";
            string uri = $"{_baseUri}/channels.history?token={_token}&channel={channel}&latest={latest}";

            using (var httpClient = new HttpClient())
            {
                using(var request = new HttpRequestMessage(new HttpMethod(requestType), uri))
                {
                    var response = await httpClient.SendAsync(request);
                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    if(response.StatusCode == HttpStatusCode.OK)
                    {
                        Messages messages =  JsonConvert.DeserializeObject<Messages>(responseBody);
                        if(messages.ok)
                        {
                            return messages;
                        }
                        else
                        {
                            throw new Exception($"Unable to get users. Response: {messages.error}");
                        }
                    }    
                    else
                    {
                        throw new Exception($"Unable to get users. Response: {responseBody}");
                    }           
                }
            }
        }

        /// <summary>
        /// A Function that download a file from slack
        /// </summary>
        /// <param name="uri"> Uri of file </param>
        /// <param name="name"> Name to save as </param>
        /// <returns>Messages</returns>

        public Stream DownloadFile(string uri, string name)
        {
            _logger.WriteLine("file-download", Severity.Info, DateTime.Now, $"Downloading File: {name}");
            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("GET"), uri))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                    HttpResponseMessage response =  httpClient.SendAsync(request).Result;

                    //Successful response
                    if(response.IsSuccessStatusCode)
                    {
                        _logger.WriteLine("file-download", Severity.Info, DateTime.Now, $"Download for File: {name} complete");
                        return response.Content.ReadAsStreamAsync().Result;
                    }
                    else
                    {
                        //Gets the response text and throws an exception
                        string error = response.Content.ReadAsStringAsync().Result;
                        _logger.WriteLine("file-download", Severity.Error, DateTime.Now, $"Error Downloading File: {name} => {error}");
                        throw new Exception(error);
                    }

                }
            }
        }

    }

    public class SlackRequest
    {
        
        private string _token;
        private string _channel;
        private string _text;

        public SlackRequest(string token, string channel, string text)
        {
            _token = token;
            _channel = channel;
            _text = text;
        }

        public MultipartFormDataContent getFormData()
        {
            return new MultipartFormDataContent()
            {
                {new StringContent(_token), "token"},
                {new StringContent(_channel), "channel"},
                {new StringContent(_text), "text"}
            };
        }
    }
}