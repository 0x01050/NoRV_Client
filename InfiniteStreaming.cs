using Google.Apis.Auth.OAuth2;
using Google.Cloud.Speech.V1;
using Google.Protobuf;
using Grpc.Auth;
using Grpc.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleTranscribing
{

    class InfiniteStreaming
    {
        private const int SampleRate = 16000;
        private const int ChannelCount = 1;
        private const int BytesPerSample = 2;
        private const int BytesPerSecond = SampleRate * ChannelCount * BytesPerSample;
        private static readonly TimeSpan s_streamTimeLimit = TimeSpan.FromSeconds(290);

        private readonly SpeechClient _client;
        public bool isRunning = false;

        public delegate void RecognizingCallback(string transcript);
        private static event RecognizingCallback Recognizing;

        public delegate void RecognizedCallback(string transcript, List<string>[] candidates);
        private static event RecognizedCallback Recognized;

        private readonly BlockingCollection<ByteString> _microphoneBuffer = new BlockingCollection<ByteString>();
        private readonly LinkedList<ByteString> _processingBuffer = new LinkedList<ByteString>();
        private TimeSpan _processingBufferStart;
        private SpeechClient.StreamingRecognizeStream _rpcStream;
        private DateTime _rpcStreamDeadline;
        private ValueTask<bool> _serverResponseAvailableTask;

        private InfiniteStreaming()
        {
            var builder = new SpeechClientBuilder();
            builder.CredentialsPath = "NoRV TTS-c4a3e2c55a4f.json";
            _client = builder.Build();
            isRunning = true;
        }

        ~InfiniteStreaming()
        {
            isRunning = false;
        }

        private async Task RunAsync(CancellationTokenSource cts)
        {
            while (isRunning && !cts.IsCancellationRequested)
            {
                await MaybeStartStreamAsync();
                if (!ProcessResponses())
                {
                    return;
                }
                await TransferMicrophoneChunkAsync();
            }
        }

        private async Task MaybeStartStreamAsync()
        {
            var now = DateTime.UtcNow;
            if (_rpcStream != null && now >= _rpcStreamDeadline)
            {
                Console.WriteLine($"Closing stream before it times out");
                await _rpcStream.WriteCompleteAsync();
                _rpcStream.GrpcCall.Dispose();
                _rpcStream = null;
            }

            if (_rpcStream != null)
            {
                return;
            }

            _rpcStream = _client.StreamingRecognize();
            _rpcStreamDeadline = now + s_streamTimeLimit;
            _processingBufferStart = TimeSpan.Zero;
            _serverResponseAvailableTask = _rpcStream.GetResponseStream().MoveNextAsync();
            await _rpcStream.WriteAsync(new StreamingRecognizeRequest
            {
                StreamingConfig = new StreamingRecognitionConfig
                {
                    Config = new RecognitionConfig
                    {
                        Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                        SampleRateHertz = SampleRate,
                        LanguageCode = "en-US",
                        MaxAlternatives = 5,
                        EnableWordTimeOffsets = true,
                        EnableAutomaticPunctuation = true
                    },
                    InterimResults = true,
                }
            });

            Console.WriteLine($"Writing {_processingBuffer.Count} chunks into the new stream.");
            foreach (var chunk in _processingBuffer)
            {
                await WriteAudioChunk(chunk);
            }
        }

        private bool ProcessResponses()
        {
            while (_serverResponseAvailableTask.IsCompleted && _serverResponseAvailableTask.Result)
            {
                var response = _rpcStream.GetResponseStream().Current;
                _serverResponseAvailableTask = _rpcStream.GetResponseStream().MoveNextAsync();
                
                var finalResult = response.Results.FirstOrDefault();
                if (finalResult != null)
                {
                    var mainAlt = finalResult.Alternatives.FirstOrDefault();
                    if (mainAlt != null)
                    {
                        if (finalResult.IsFinal)
                        {
                            if(Recognized == null)
                                Console.WriteLine($"\nTranscript: {mainAlt.Transcript}\n");
                            else
                            {
                                int wordCnt = mainAlt.Transcript.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                                List<string>[] candidates = new List<string>[wordCnt];
                                for (int i = 0; i < wordCnt; i++)
                                {
                                    candidates[i] = new List<string>();
                                }
                                foreach (var alt in finalResult.Alternatives)
                                {
                                    var words = alt.Transcript.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    if(wordCnt == words.Length)
                                    {
                                        for (int i = 0; i < wordCnt; i++)
                                        {
                                            if (!candidates[i].Contains(words[i]))
                                            {
                                                candidates[i].Add(words[i]);
                                            }
                                        }
                                    }
                                }
                                Recognized(mainAlt.Transcript, candidates);
                            }
                        }
                        else
                        {
                            if (Recognizing == null)
                                Console.WriteLine($"\t\tPre-Transcript: {mainAlt.Transcript}");
                            else
                                Recognizing(mainAlt.Transcript);
                        }

                    }

                    TimeSpan resultEndTime = finalResult.ResultEndTime.ToTimeSpan();

                    int removed = 0;
                    while (_processingBuffer.First != null)
                    {
                        var sampleDuration = TimeSpan.FromSeconds(_processingBuffer.First.Value.Length / (double)BytesPerSecond);
                        var sampleEnd = _processingBufferStart + sampleDuration;

                        if (sampleEnd > resultEndTime)
                        {
                            break;
                        }
                        _processingBufferStart = sampleEnd;
                        _processingBuffer.RemoveFirst();
                        removed++;
                    }
                }
            }
            return true;
        }

        private async Task TransferMicrophoneChunkAsync()
        {
            var chunk = _microphoneBuffer.Take();
            _processingBuffer.AddLast(chunk);
            await WriteAudioChunk(chunk);
        }

        private Task WriteAudioChunk(ByteString chunk) =>
            _rpcStream.WriteAsync(new StreamingRecognizeRequest { AudioContent = chunk });

        private async Task ListenAudio(Socket handler, int number)
        {
            byte[] bytes = null;
            while (isRunning)
            {
                bytes = new byte[1024];
                int bytesRec = handler.Receive(bytes);
                if(bytesRec > 0)
                    _microphoneBuffer.Add(ByteString.CopyFrom(bytes, 0, bytesRec));
            }
            handler.Close();
        }

        private async Task StartListening(CancellationTokenSource cts, Socket handler)
        {
            Task.Run(() => ListenAudio(handler, new Random().Next(0, 100)), cts.Token);
            try
            {
                await RunAsync(cts);
            }
            catch (Exception) {}
            isRunning = false;
        }

        public static async Task RecognizeAsync(CancellationTokenSource cts, RecognizedCallback recognized = null, RecognizingCallback recognizing = null)
        {
            Recognized = recognized;
            Recognizing = recognizing;

            IPAddress ipAddress = IPAddress.Loopback;
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 15980);

            try
            {
                using (Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    listener.Bind(localEndPoint);
                    listener.Listen(10);

                    InfiniteStreaming instance = null;
                    while (!cts.IsCancellationRequested)
                    {
                        Socket handler = listener.Accept();
                        if (instance != null)
                            instance.isRunning = false;
                        instance = new InfiniteStreaming();
                        try
                        {
                            Task.Run(() => instance.StartListening(cts, handler), cts.Token);
                        }
                        catch(Exception)
                        {
                            instance.isRunning = false;
                        }
                    }

                    if (instance != null)
                        instance.isRunning = false;
                    listener.Shutdown(SocketShutdown.Both);
                    listener.Disconnect(true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
