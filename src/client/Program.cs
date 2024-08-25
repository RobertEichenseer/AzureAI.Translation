string filePath = "../../assets/input/en-sample.wav";
string url = "https://localhost:8080/api/StreamAudio/stream";

using (HttpClient client = new HttpClient())
{
    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
    {
        using (StreamContent content = new StreamContent(fs))
        {
            content.Headers.Add("Content-Type", "audio/wav");

            HttpResponseMessage response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("File uploaded successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to upload file. Status code: {response.StatusCode}");
            }
        }
    }
}
