using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using Streaminvi.Properties;
using TweetinCore.Enum;
using TweetinCore.Events;
using TweetinCore.Interfaces.StreamInvi;

namespace Streaminvi.Helpers
{
    /// <summary>
    /// Extract objects from any kind of stream
    /// </summary>
    public class StreamResultGenerator : IStreamResultGenerator
    {
        private const int STREAM_RESUME_DELAY = 1000;

        public event EventHandler StreamStarted;
        public event EventHandler StreamResumed;
        public event EventHandler StreamPaused;
        public event EventHandler<GenericEventArgs<Exception>> StreamStopped;

        private StreamReader _currentReader;
        private Exception _lastException;

        private bool IsRunning
        {
            get { return _streamState == StreamState.Resume || _streamState == StreamState.Pause; }
        }

        private StreamState _streamState;
        public StreamState StreamState
        {
            get { return _streamState; }
            set
            {
                if (_streamState != value)
                {
                    _streamState = value;

                    switch (_streamState)
                    {
                        case StreamState.Resume:
                            this.Raise(StreamResumed);
                            break;
                        case StreamState.Pause:
                            this.Raise(StreamPaused);
                            break;
                        case StreamState.Stop:
                            this.Raise(StreamStopped, _lastException);
                            break;
                    }
                }
            }
        }

        public void StartStream(Func<string, bool> processObject, Func<HttpWebRequest> generateWebRequest)
        {
            if (IsRunning)
            {
                throw new OperationCanceledException(Resources.Stream_IllegalMultipleStreams);
            }

            if (processObject == null)
            {
                throw new NullReferenceException(Resources.Stream_ObjectDelegateIsNull);
            }

            #region Variables
            _lastException = null;
            HttpWebRequest webRequest = generateWebRequest();
            webRequest.Timeout = -1;
            _currentReader = InitWebRequest(webRequest);

            if (_lastException != null && StreamStopped == null)
            {
                 throw _lastException;
            }

            this.Raise(StreamStarted);
            _streamState = StreamState.Resume;

            int errorOccured = 0;
            #endregion

            while (StreamState != StreamState.Stop)
            {
                if (StreamState == StreamState.Pause)
                {
                    Thread.Sleep(STREAM_RESUME_DELAY);
                    continue;
                }
                
                try
                {
                    string jsonResponse = _currentReader.ReadLine();
                    #region Error Checking

                    if (jsonResponse == null)
                    {
                        if (errorOccured == 0)
                        {
                            ++errorOccured;
                        }
                        else if (errorOccured == 1)
                        {
                            ++errorOccured;
                            webRequest.Abort();
                            _currentReader = InitWebRequest(webRequest);
                        }
                        else if (errorOccured == 2)
                        {
                            ++errorOccured;
                            webRequest.Abort();
                            webRequest = generateWebRequest();
                            _currentReader = InitWebRequest(webRequest);
                        }
                        else
                        {
                            Console.WriteLine("Twitter API is not accessible");
                            Trace.WriteLine("Twitter API is not accessible");
                            break;
                        }
                    }
                    else
                    {
                        errorOccured = 0;
                    }

                    #endregion

                    if (StreamState == StreamState.Resume && !processObject(jsonResponse))
                    {
                        StreamState = StreamState.Stop;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _lastException = ex;

                    if (ex is IOException)
                    {
                        if (StreamState == StreamState.Stop)
                        {
                            return;
                        }

                        // Verify the implementation of the Exception handler
                        #region IOException Handler

                        if (ex.Message == "Unable to read data from the transport connection: The connection was closed.")
                        {
                            _currentReader = InitWebRequest(webRequest);
                        }

                        try
                        {
                            _currentReader.ReadLine();
                            _lastException = null;
                        }
                        catch (IOException ex2)
                        {
                            if (ex2.Message ==
                                "Unable to read data from the transport connection: The connection was closed.")
                            {
                                Trace.WriteLine("Streamreader was unable to read from the stream!");
                            }
                        }
                        catch (ObjectDisposedException)
                        {
                            // StopStream has been called
                            _lastException = null;
                        }
                        #endregion
                    }

                    break;
                }
            }

            #region Clean

            if (webRequest != null)
            {
                webRequest.Abort();
            }

            if (_currentReader != null)
            {
                _currentReader.Dispose();
            }

            StreamState = StreamState.Stop;
            #endregion
        }

        private StreamReader InitWebRequest(WebRequest webRequest)
        {
            StreamReader reader = null;

            try
            {
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                Stream responseStream = webResponse.GetResponseStream();

                if (responseStream != null)
                {
                    reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
                }
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException)
                {
                    if (ex.Message == "Stream was not readable.")
                    {
                        webRequest.Abort();
                    }
                }

                _lastException = ex;
                StreamState = StreamState.Stop;
            }

            return reader;
        }

        public void ResumeStream()
        {
            StreamState = StreamState.Resume;
        }

        public void PauseStream()
        {
            StreamState = StreamState.Pause;
        }

        public void StopStream()
        {
            StreamState = StreamState.Stop;
            if (_currentReader != null)
            {
                _currentReader.Close();
            }
        }
    }
}
