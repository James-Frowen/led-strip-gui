using NAudio.Wave;
using System;

namespace interactive_audio
{
    public interface IAudioRenderer : IDisposable
    {
        void OnData(WaveInEventArgs a);
    }
}