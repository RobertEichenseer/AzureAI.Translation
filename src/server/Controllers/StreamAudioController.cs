using Microsoft.AspNetCore.Mvc;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Translation;
using Microsoft.CognitiveServices.Speech.Audio;

[ApiController]
[Route("api/[controller]")]
public  class StreamAudioController : ControllerBase
{
    HttpClient httpClient = new HttpClient(); 

    [HttpPost("Stream")]
    public async Task<IActionResult> StreamAudio()
    {
        string speechApiKey = Environment.GetEnvironmentVariable("SPEECH_APIKEY") ?? "";
        string speechRegion = Environment.GetEnvironmentVariable("SPEECH_REGION") ?? "";
        
        byte channels = 1;
        byte bitsPerSample = 16;
        uint samplesPerSecond = 16000; // or 8000

        AudioStreamFormat audioFormat = AudioStreamFormat.GetWaveFormatPCM(samplesPerSecond, bitsPerSample, channels);
        AudioConfig audioConfig = AudioConfig.FromStreamInput(new CustomAudioStream(HttpContext.Request.Body), audioFormat);

        SpeechTranslationConfig speechTranslationConfig = SpeechTranslationConfig.FromSubscription(speechApiKey, speechRegion);
        speechTranslationConfig.SpeechRecognitionLanguage = "en-US";

        string[] targetLanguages = new string[]{"de", "it"};
        targetLanguages.ToList<string>().ForEach(targetLanguage => speechTranslationConfig.AddTargetLanguage(targetLanguage));
        
        using TranslationRecognizer translationRecognizer = new TranslationRecognizer(speechTranslationConfig, audioConfig);
        translationRecognizer.Recognizing += TranslationRecognizing; 
        translationRecognizer.Recognized += TranslationRecognized; 
        translationRecognizer.Canceled += TranslationCanceled; 
        translationRecognizer.SessionStopped += TranslationSessionStopped;
        
        TranslationRecognitionResult translationRecognitionResult = await translationRecognizer.RecognizeOnceAsync();

        return Ok();
    }

    private void TranslationRecognizing (object? s, TranslationRecognitionEventArgs e) 
    {
        string translation = ""; 
        foreach (KeyValuePair<string,string> keyValuePair in e.Result.Translations)
        {
            translation = string.Concat(translation, $"Translation: '{keyValuePair.Key}': {keyValuePair.Value} \n\n");
        }
        httpClient.PostAsync("https://localhost:8081/api/showtranscription", new StringContent(translation));     
    }

    private void TranslationRecognized (object? s, TranslationRecognitionEventArgs speechRecognitionEventArgs) 
    {
        TranslationRecognitionResult translationRecognitionResult = speechRecognitionEventArgs.Result; 

        if (translationRecognitionResult.Reason == ResultReason.TranslatedSpeech) 
        {
            string translation = ""; 
            foreach (KeyValuePair<string,string> keyValuePair in translationRecognitionResult.Translations)
            {
                translation = string.Concat(translation, $"Translation: '{keyValuePair.Key}': {keyValuePair.Value} \n\n");
            }
            httpClient.PostAsync("https://localhost:8081/api/showtranscription", new StringContent(translation));     
        } 
    }

    private void TranslationCanceled (object? s, TranslationRecognitionCanceledEventArgs e) 
    {
        Console.WriteLine($"Canceled: {e.Reason}");
        if (e.Reason == CancellationReason.Error) 
        {
            Console.WriteLine($"Canceled: {e.ErrorDetails}"); 
        }
    }
    private void TranslationSessionStopped(object? sender, SessionEventArgs e)
    {
        Console.WriteLine($"Stopped"); 
    }

}