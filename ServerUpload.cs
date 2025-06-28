using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp
{
    public class ServerUpload
    {
        public event Action<string> SketchUploaded;

        private const int Port = 5000;

        public async Task StartAsync()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();

                if (ServerWindow.TokenSource.IsCancellationRequested)
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
            {
                string request;
                using (var ms = new MemoryStream())
                {
                    byte[] buffer = new byte[8192];
                    int bytesRead;
                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, bytesRead);
                    }

                    request = Encoding.UTF8.GetString(ms.ToArray());
                }


                if (request.StartsWith("SAVE_AS:"))
                {
                    int newlineIndex = request.IndexOf('\n');
                    if (newlineIndex == -1)
                        return;

                    string fileName = request.Substring(8, newlineIndex - 8).Trim();
                    string json = request.Substring(newlineIndex + 1);

                    string folder = Path.Combine(Directory.GetCurrentDirectory(), "Sketches");
                    Directory.CreateDirectory(folder);
                    string filePath = Path.Combine(folder, fileName + ".json");
                    File.WriteAllText(filePath, json);



                    SketchUploaded?.Invoke(fileName);

                }
            }

            client.Close();
        }
    }
}
