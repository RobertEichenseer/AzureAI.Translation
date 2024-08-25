using System.IO;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

public class CustomAudioStream : PullAudioInputStreamCallback 
{

    private readonly Stream _stream; 

    public CustomAudioStream(Stream _stream) {
        this._stream = _stream;
    }

    public override int Read(byte[] buffer, uint size) 
    {
        return _stream.Read(buffer, 0, (int)size);
    }

    public override void Close() 
    {
        _stream.Close(); 
    }
}