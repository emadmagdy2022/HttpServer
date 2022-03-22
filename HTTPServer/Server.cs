using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;
        IPEndPoint iPEndPoint;
        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            iPEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(iPEndPoint);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            Console.WriteLine("Start listening...");
            serverSocket.Listen(50);

            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clintSocket = serverSocket.Accept();
                Thread thread = new Thread(new ParameterizedThreadStart(HandleConnection));
                thread.Start(clintSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            Console.WriteLine("Connected");
            // TODO: Create client socket 
            Socket clintSocket = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clintSocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] receiveRequist = new byte[50000];
                    int receiveRequistLength = clintSocket.Receive(receiveRequist);
                    string request = Encoding.ASCII.GetString(receiveRequist);

                    Console.WriteLine(request);
                    // TODO: break the while loop if receivedLen==0
                    if (receiveRequistLength == 0)
                        break;
                    // TODO: Create a Request object using received request string
                    Request clintReq = new Request(request);
                    // TODO: Call HandleRequest Method that returns the response
                    Response ServerResponse = HandleRequest(clintReq);
                    string resp = ServerResponse.ResponseString;
                    Console.WriteLine(resp);

                    byte[] response = Encoding.ASCII.GetBytes(resp);
                    // TODO: Send Response back to client
                    clintSocket.Send(response);

                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clintSocket.Shutdown(SocketShutdown.Both);
            clintSocket.Close();
        }



        Response HandleRequest(Request request)
        {
            string content = "";
            string path;
            try
            {
                //TODO: check for bad request 
                if(!request.ParseRequest())
                {
                    path = Configuration.RootPath+ '\\' + "BadRequest.html";

                    content = File.ReadAllText(path);
                    Response response = new Response(StatusCode.BadRequest, "text/html", content,path);
                    return response;
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.
                string[] name = request.relativeURI.Split('/');
                string physical_path = Configuration.RootPath + '\\' + name[1];
                //TODO: check for redirect
                physical_path = GetRedirectionPagePathIFExist(request.relativeURI);
                if(!string.IsNullOrWhiteSpace(physical_path))
                {
                    Response res = new Response(StatusCode.Redirect, "text/html", content, physical_path);
                }
                
                //TODO: check file exists
                if (!File.Exists(physical_path))
                {
                    physical_path = Configuration.RootPath + '\\' + "NotFound.html";
                    content = File.ReadAllText(physical_path);
                    return new Response(StatusCode.NotFound, "text/html", content, physical_path);
                }
                //TODO: read the physical file
                else
                {
                    content = File.ReadAllText(physical_path);
                }

                // Create OK response
                return new Response(StatusCode.OK, "text/html", content, physical_path);
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                string physical_path = Configuration.RootPath + '\\' + "InternalError.html";
                content = File.ReadAllText(physical_path);
                Response response = new Response(StatusCode.InternalServerError, "text/html", content, physical_path);
                return response;
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            for (int i = 0; i < Configuration.RedirectionRules.Count; i++)
            {
                if(relativePath=='/'+Configuration.RedirectionRules.Keys.ElementAt(i).ToString())
                {
                    string redirectedPath = Configuration.RedirectionRules.Values.ElementAt(i).ToString();
                    string physicalPath = Configuration.RootPath + '\\' + redirectedPath;
                    return physicalPath;
                }
            }
            return string.Empty;
        }


        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            string content = "";
            try
            {
                // TODO: check if filepath not exist log exception using Logger class and return empty string
                if (File.Exists(filePath))
                {
                    // else read file and return its content
                    content = File.ReadAllText(filePath);
                }
            }

            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return content;

        }
        private void LoadRedirectionRules(string filePath)
        {
            string line;
            try
            {
                
                StreamReader streamReader = new StreamReader(filePath);
                Configuration.RedirectionRules = new Dictionary<string, string>();
                // TODO: using the filepath paramter read the redirection rules from file 
                while ((line =streamReader.ReadLine()) != null)
                {
                    
                    string[] data = line.Split(',');
                    
                    Configuration.RedirectionRules.Add(data[0], data[1]);
                }
                
                    
                // then fill Configuration.RedirectionRules dictionary 
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
                
            }
        }
    }
}
