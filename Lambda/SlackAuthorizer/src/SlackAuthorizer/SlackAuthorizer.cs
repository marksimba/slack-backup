using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Slack.DataModels;
using Amazon.Lambda.APIGatewayEvents;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Lambda
{
    public class SlackAuthorizer
    {
        private readonly Logger _logger;
        private readonly int _requestWindow = 5;
        public SlackAuthorizer()
        {
            _logger = new Logger();
        }
        
        /// <summary>
        /// Unfortunately we are unable to utilize this as a true Authorizer.
        /// This is due to the fact that AWS Authorizers are not privy to the request boy.
        /// Slack requires the body to verify the signature.
        /// What we will do here, is verify the following: 
        /// - The Header "X-Slack-Request-Timestamp" exists and was within the last 5 minutes.
        /// - The Header "X-Slack-Signature" exists and has the correct version.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public APIGatewayCustomAuthorizerResponse VerifyCorrectHeaders(Authorizer input, ILambdaContext context)
        {
            //default to not allow
            bool allow = false;

            string signatureVersion = Environment.GetEnvironmentVariable("SLACKSIGNATUREVERSION");
            string signature = input.headers.ContainsKey("X-Slack-Signature") ? input.headers["X-Slack-Signature"] : null;
            string timeStamp = input.headers.ContainsKey("X-Slack-Request-Timestamp") ? input.headers["X-Slack-Request-Timestamp"] : null;

            //Verifies the signature and timeStamp exists
            if(!string.IsNullOrEmpty(signature) && !string.IsNullOrEmpty(timeStamp))
            {
                //Verifies signature has the correct version.
                if(signature.StartsWith(signatureVersion))
                {
                    //Verifies timestamp is valid
                    DateTime currentTime = DateTime.UtcNow;
                    try
                    {
                        DateTime requestTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(timeStamp)).UtcDateTime;

                        //Verifies the request was within the last request window
                        if(requestTime > currentTime.Add(new TimeSpan(0,-5,0)) && requestTime < DateTime.UtcNow)
                        {
                            //If it means all the above conditions, we have the correct headers/values
                            allow = true;
                        }
                        else
                        {
                            _logger.WriteLine("slack-authorizer", Severity.Error, DateTime.Now, "Timestamp is not within the appropriate timeframe");
                        }
                    }
                    catch(Exception e)
                    {
                        _logger.WriteLine("slack-authorizer", Severity.Error, DateTime.Now, "Invalid TimeStamp");
                    }
                }
                else
                {
                    _logger.WriteLine("slack-authorizer", Severity.Error, DateTime.Now, "Incorrect Signature Version.");
                }
            }
            else
            {
                _logger.WriteLine("slack-authorizer", Severity.Error, DateTime.Now, "Signature and/or Timestamp headers don't exist");
            }

            APIGatewayCustomAuthorizerResponse response = new APIGatewayCustomAuthorizerResponse
		    {
                PrincipalID = timeStamp,
			    PolicyDocument = new APIGatewayCustomAuthorizerPolicy
			    {
				    Version = "2012-10-17",
				    Statement = new List<APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement>
				    {
					    new APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement
					    {
						    Action = new HashSet<string> { "execute-api:Invoke" },
						    Effect = allow ? "Allow" : "Deny",
						    Resource = new HashSet<string> { input.methodArn }
					    }
				    }
			    }
		    };

            if(allow)
            {
                _logger.WriteLine("slack-authorizer", Severity.Info, DateTime.Now, "Request Allowed");
            }
            else
            {
                _logger.WriteLine("slack-authorizer", Severity.Error, DateTime.Now, "Request Denied");
            }

            // Return the message to allow/deny this API request
			return response;
        }
    }
}
