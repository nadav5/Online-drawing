using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SketchServer
{
    public class ServerDownload
    {
        private const int Port = 5001;

        public async Task StartAsync()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();

                if (FirstApp.ServerWindow.TokenSource.IsCancellationRequested)
                {
                    client.Close(); 
                    continue;
                }

                _ = HandleClientAsync(client);
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (NetworkStream stream = client.GetStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
            {
                string request = await reader.ReadLineAsync();

                string folder = Path.Combine(Directory.GetCurrentDirectory(), "Sketches");
                Directory.CreateDirectory(folder);

                if (request == "GET_FILE_LIST")
                {
                    var files = Directory.GetFiles(folder, "*.json")
                                         .Select(Path.GetFileName)
                                         .ToList();

                    string jsonList = System.Text.Json.JsonSerializer.Serialize(files);
                    await writer.WriteLineAsync(jsonList);
                }
                else if (request.StartsWith("GET_FILE:"))
                {
                    string fileName = request.Substring("GET_FILE:".Length).Trim();
                    string filePath = Path.Combine(folder, fileName);
                    if (File.Exists(filePath))
                    {
                        string content = File.ReadAllText(filePath);
                        await writer.WriteLineAsync(content);
                    }
                    else
                    {
                        await writer.WriteLineAsync("ERROR: File not found");
                    }
                }
            }

            client.Close();
        }
    }
}
