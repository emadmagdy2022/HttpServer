using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            //throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            headerLines.Add("Content-Type:" + contentType);
            headerLines.Add("Content-Length:" + content.Length);
            headerLines.Add("Date:" + DateTime.Now);
            headerLines.Add("Redirect-Location" + redirectoinPath);

            // TODO: Create the request string
            //reques consists of :
            //1)status line
            string StatusLine = GetStatusLine(code);
            //2)hearline
            string HeaderLine = "";
            //loop on the headerlineslist 
            for(int i=0;i<headerLines.Count;i++)
            {
                HeaderLine += headerLines[i];
                HeaderLine += "\r\n";
            }
            //3)blank line
            string BlankLine = "\r\n";
            //4)content
            string Content = content;
            responseString = StatusLine + "\r\n" + HeaderLine + BlankLine + Content;


        }

        private string GetStatusLine(StatusCode code)
        {

            // TODO: Create the response status line and return it
            string statusLine = string.Empty;
            //ststus line consists of 3 parts:
            //1)http version number
            string httpVersion = Configuration.ServerHTTPVersion;
            statusLine = httpVersion + " ";
            //2)status code
            statusLine += ((int)code).ToString() + " ";
            //3)status text
            statusLine += code.ToString();
            return statusLine;
        }
    }
}
