using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //throw new NotImplementedException();
            bool checkRequest = true;
            //TODO: parse the receivedRequest using the \r\n delimeter   
            string[] charSplit = { "\r\n" };
            contentLines = requestString.Split(charSplit, StringSplitOptions.None);
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (contentLines.Length < 3)
            {
                return false;
            }
            // Parse Request line
            requestLines = contentLines[0].Split(' ');
            checkRequest &= ParseRequestLine();
            // Validate blank line exists
            checkRequest &= ValidateBlankLine();
            // Load header lines into HeaderLines dictionar
            checkRequest &= LoadHeaderLines();
            return checkRequest;
        }

        private bool ParseRequestLine()
        {
            //check if lines lower than 2 return false (Bad Request)
            if (requestLines.Length < 2)
                return false;
            //1)Method
            if (requestLines[0].ToUpper() == "GET")
                method = RequestMethod.GET;
            else if (requestLines[0].ToUpper() == "HEAD")
                method = RequestMethod.HEAD;
            else if (requestLines[0].ToUpper() == "POST")
                method = RequestMethod.POST;
            else
                return false;
            //2)URI(URL)
            relativeURI = requestLines[1];
            //3)Version
            if (requestLines[2] == "HTTP/1.1")
                httpVersion = HTTPVersion.HTTP11;
            else if (requestLines[2] == "HTTP/1.0")
                httpVersion = HTTPVersion.HTTP10;
            else
                httpVersion = HTTPVersion.HTTP09;
            //check validateIs URI return true or not   
            return ValidateIsURI(relativeURI);

        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            //throw new NotImplementedException();
            bool check = true;
            headerLines = new Dictionary<string, string>();
            for (int i = 1; i < contentLines.Length - 2; i++)
            {
                if (contentLines[i].Contains(":"))
                {
                    string[] splitChar = { ": " };
                    string[] requestHeader = contentLines[i].Split(splitChar, StringSplitOptions.None);
                    headerLines.Add(requestHeader[0], requestHeader[1]);
                    //if(requestHeader[0].Contains("Content-Length"))

                }
                else check = false;
            }
            return check;
        }

        private bool ValidateBlankLine()
        {
            //throw new NotImplementedException();
            //check BlankLine length-2 ->([[length-1]-1]
            if (contentLines[(contentLines.Length - 2)] == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
