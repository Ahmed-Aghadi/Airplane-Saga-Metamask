using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;

#if UNITY_WEBGL && !UNITY_EDITOR
using AOT;
using System.Runtime.InteropServices;
#endif

namespace MetaMask.NativeWebSocket
{

    public class MainThreadUtil : MonoBehaviour
    {
        public static MainThreadUtil Instance { get; private set; }
        public static SynchronizationContext synchronizationContext { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Setup()
        {
            Instance = new GameObject("MainThreadUtil")
                .AddComponent<MainThreadUtil>();
            synchronizationContext = SynchronizationContext.Current;
        }

        public static void Run(IEnumerator waitForUpdate)
        {
            synchronizationContext.Post(_ => Instance.StartCoroutine(
                        waitForUpdate), null);
        }

        private void Awake()
        {
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(gameObject);
        }
    }

    public class WaitForUpdate : CustomYieldInstruction
    {
        public override bool keepWaiting
        {
            get { return false; }
        }

        public MainThreadAwaiter GetAwaiter()
        {
            var awaiter = new MainThreadAwaiter();
            MainThreadUtil.Run(CoroutineWrapper(this, awaiter));
            return awaiter;
        }

        public class MainThreadAwaiter : INotifyCompletion
        {
            private Action continuation;

            public bool IsCompleted { get; set; }

            public void GetResult() { }

            public void Complete()
            {
                IsCompleted = true;
                this.continuation?.Invoke();
            }

            void INotifyCompletion.OnCompleted(Action continuation)
            {
                this.continuation = continuation;
            }
        }

        public static IEnumerator CoroutineWrapper(IEnumerator theWorker, MainThreadAwaiter awaiter)
        {
            yield return theWorker;
            awaiter.Complete();
        }
    }

    public delegate void WebSocketOpenEventHandler();
    public delegate void WebSocketMessageEventHandler(byte[] data);
    public delegate void WebSocketTextMessageEventHandler(string data);
    public delegate void WebSocketErrorEventHandler(string errorMsg);
    public delegate void WebSocketCloseEventHandler(WebSocketCloseCode closeCode);

    public enum WebSocketCloseCode
    {
        /* Do NOT use NotSet - it's only purpose is to indicate that the close code cannot be parsed. */
        NotSet = 0,
        Normal = 1000,
        Away = 1001,
        ProtocolError = 1002,
        UnsupportedData = 1003,
        Undefined = 1004,
        NoStatus = 1005,
        Abnormal = 1006,
        InvalidData = 1007,
        PolicyViolation = 1008,
        TooBig = 1009,
        MandatoryExtension = 1010,
        ServerError = 1011,
        TlsHandshakeFailure = 1015
    }

    public enum WebSocketState
    {
        Connecting,
        Open,
        Closing,
        Closed
    }

    public interface IWebSocket
    {
        event WebSocketOpenEventHandler OnOpen;
        event WebSocketMessageEventHandler OnMessage;
        event WebSocketTextMessageEventHandler OnTextMessage;
        event WebSocketErrorEventHandler OnError;
        event WebSocketCloseEventHandler OnClose;

        WebSocketState State { get; }
    }

    public static class WebSocketHelpers
    {
        public static WebSocketCloseCode ParseCloseCodeEnum(int closeCode)
        {

            if (WebSocketCloseCode.IsDefined(typeof(WebSocketCloseCode), closeCode))
            {
                return (WebSocketCloseCode)closeCode;
            }
            else
            {
                return WebSocketCloseCode.Undefined;
            }

        }

        public static WebSocketException GetErrorMessageFromCode(int errorCode, Exception inner)
        {
            switch (errorCode)
            {
                case -1:
                    return new WebSocketUnexpectedException("WebSocket instance not found.", inner);
                case -2:
                    return new WebSocketInvalidStateException("WebSocket is already connected or in connecting state.", inner);
                case -3:
                    return new WebSocketInvalidStateException("WebSocket is not connected.", inner);
                case -4:
                    return new WebSocketInvalidStateException("WebSocket is already closing.", inner);
                case -5:
                    return new WebSocketInvalidStateException("WebSocket is already closed.", inner);
                case -6:
                    return new WebSocketInvalidStateException("WebSocket is not in open state.", inner);
                case -7:
                    return new WebSocketInvalidArgumentException("Cannot close WebSocket. An invalid code was specified or reason is too long.", inner);
                default:
                    return new WebSocketUnexpectedException("Unknown error.", inner);
            }
        }
    }

    public class WebSocketException : Exception
    {
        public WebSocketException() { }
        public WebSocketException(string message) : base(message) { }
        public WebSocketException(string message, Exception inner) : base(message, inner) { }
    }

    public class WebSocketUnexpectedException : WebSocketException
    {
        public WebSocketUnexpectedException() { }
        public WebSocketUnexpectedException(string message) : base(message) { }
        public WebSocketUnexpectedException(string message, Exception inner) : base(message, inner) { }
    }

    public class WebSocketInvalidArgumentException : WebSocketException
    {
        public WebSocketInvalidArgumentException() { }
        public WebSocketInvalidArgumentException(string message) : base(message) { }
        public WebSocketInvalidArgumentException(string message, Exception inner) : base(message, inner) { }
    }

    public class WebSocketInvalidStateException : WebSocketException
    {
        public WebSocketInvalidStateException() { }
        public WebSocketInvalidStateException(string message) : base(message) { }
        public WebSocketInvalidStateException(string message, Exception inner) : base(message, inner) { }
    }

    public class WaitForBackgroundThread
    {
        public ConfiguredTaskAwaitable.ConfiguredTaskAwaiter GetAwaiter()
        {
            return Task.Run(() => { }).ConfigureAwait(false).GetAwaiter();
        }
    }

#if UNITY_WEBGL && !UNITY_EDITOR

    /// <summary>
    /// WebSocket class bound to JSLIB.
    /// </summary>
    public class WebSocket : IWebSocket
    {

        /* WebSocket JSLIB functions */
        [DllImport("__Internal")]
        public static extern int WebSocketConnect(int instanceId);

        [DllImport("__Internal")]
        public static extern int WebSocketClose(int instanceId, int code, string reason);

        [DllImport("__Internal")]
        public static extern int WebSocketSend(int instanceId, byte[] dataPtr, int dataLength);

        [DllImport("__Internal")]
        public static extern int WebSocketSendText(int instanceId, string message);

        [DllImport("__Internal")]
        public static extern int WebSocketGetState(int instanceId);

        protected int instanceId;

        public event WebSocketOpenEventHandler OnOpen;
        public event WebSocketMessageEventHandler OnMessage;
        public event WebSocketTextMessageEventHandler OnTextMessage;
        public event WebSocketErrorEventHandler OnError;
        public event WebSocketCloseEventHandler OnClose;

        public WebSocket(string url, Dictionary<string, string> headers = null)
        {
            if (!WebSocketFactory.isInitialized)
            {
                WebSocketFactory.Initialize();
            }

            int instanceId = WebSocketFactory.WebSocketAllocate(url);
            WebSocketFactory.instances.Add(instanceId, this);

            this.instanceId = instanceId;
        }

        public WebSocket(string url, string subprotocol, Dictionary<string, string> headers = null)
        {
            if (!WebSocketFactory.isInitialized)
            {
                WebSocketFactory.Initialize();
            }

            int instanceId = WebSocketFactory.WebSocketAllocate(url);
            WebSocketFactory.instances.Add(instanceId, this);

            WebSocketFactory.WebSocketAddSubProtocol(instanceId, subprotocol);

            this.instanceId = instanceId;
        }

        public WebSocket(string url, List<string> subprotocols, Dictionary<string, string> headers = null)
        {
            if (!WebSocketFactory.isInitialized)
            {
                WebSocketFactory.Initialize();
            }

            int instanceId = WebSocketFactory.WebSocketAllocate(url);
            WebSocketFactory.instances.Add(instanceId, this);

            foreach (string subprotocol in subprotocols)
            {
                WebSocketFactory.WebSocketAddSubProtocol(instanceId, subprotocol);
            }

            this.instanceId = instanceId;
        }

        ~WebSocket()
        {
            WebSocketFactory.HandleInstanceDestroy(this.instanceId);
        }

        public int GetInstanceId()
        {
            return this.instanceId;
        }

        public Task Connect()
        {
            int ret = WebSocketConnect(this.instanceId);

            if (ret < 0)
                throw WebSocketHelpers.GetErrorMessageFromCode(ret, null);

            return Task.CompletedTask;
        }

        public void CancelConnection()
        {
            if (State == WebSocketState.Open)
                Close(WebSocketCloseCode.Abnormal);
        }

        public Task Close(WebSocketCloseCode code = WebSocketCloseCode.Normal, string reason = null)
        {
            int ret = WebSocketClose(this.instanceId, (int)code, reason);

            if (ret < 0)
                throw WebSocketHelpers.GetErrorMessageFromCode(ret, null);

            return Task.CompletedTask;
        }

        public Task Send(byte[] data)
        {
            int ret = WebSocketSend(this.instanceId, data, data.Length);

            if (ret < 0)
                throw WebSocketHelpers.GetErrorMessageFromCode(ret, null);

            return Task.CompletedTask;
        }

        public Task SendText(string message)
        {
            int ret = WebSocketSendText(this.instanceId, message);

            if (ret < 0)
                throw WebSocketHelpers.GetErrorMessageFromCode(ret, null);

            return Task.CompletedTask;
        }

        public WebSocketState State
        {
            get
            {
                int state = WebSocketGetState(this.instanceId);

                if (state < 0)
                    throw WebSocketHelpers.GetErrorMessageFromCode(state, null);

                switch (state)
                {
                    case 0:
                        return WebSocketState.Connecting;

                    case 1:
                        return WebSocketState.Open;

                    case 2:
                        return WebSocketState.Closing;

                    case 3:
                        return WebSocketState.Closed;

                    default:
                        return WebSocketState.Closed;
                }
            }
        }

        public void DelegateOnOpenEvent()
        {
            OnOpen?.Invoke();
        }

        public void DelegateOnMessageEvent(byte[] data)
        {
            OnMessage?.Invoke(data);
        }

        public void DelegateOnTextMessageEvent(string data)
        {
            OnTextMessage?.Invoke(data);
        }

        public void DelegateOnErrorEvent(string errorMsg)
        {
            OnError?.Invoke(errorMsg);
        }

        public void DelegateOnCloseEvent(int closeCode)
        {
            OnClose?.Invoke(WebSocketHelpers.ParseCloseCodeEnum(closeCode));
        }

    }

//#elif UNITY_ANDROID

//    public class WebSocket : AndroidWebSocket
//    {

//        public WebSocket(string url, Dictionary<string, string> headers = null) : base(url, headers)
//        {

//        }

//        public WebSocket(string url, string subprotocol, Dictionary<string, string> headers = null) : base(url, subprotocol, headers)
//        {
//        }

//        public WebSocket(string url, List<string> subprotocols, Dictionary<string, string> headers = null) : base(url, subprotocols, headers)
//        {
//        }

//    }

#else

#if UNITY_ANDROID

    public class BackgroundWebSocket : AndroidWebSocket
    {

        public BackgroundWebSocket(string url, Dictionary<string, string> headers = null) : base(url, headers)
        {

        }

        public BackgroundWebSocket(string url, string subprotocol, Dictionary<string, string> headers = null) : base(url, subprotocol, headers)
        {
        }

        public BackgroundWebSocket(string url, List<string> subprotocols, Dictionary<string, string> headers = null) : base(url, subprotocols, headers)
        {
        }

    }

#elif UNITY_IOS

    public class BackgroundWebSocket : IosWebSocket
    {

        public BackgroundWebSocket(string url, Dictionary<string, string> headers = null) : base(url, headers)
        {

        }

        public BackgroundWebSocket(string url, string subprotocol, Dictionary<string, string> headers = null) : base(url, subprotocol, headers)
        {
        }

        public BackgroundWebSocket(string url, List<string> subprotocols, Dictionary<string, string> headers = null) : base(url, subprotocols, headers)
        {
        }

    }

#endif

    public class WebSocket : IWebSocket
    {
        public event WebSocketOpenEventHandler OnOpen;
        public event WebSocketMessageEventHandler OnMessage;
        public event WebSocketTextMessageEventHandler OnTextMessage;
        public event WebSocketErrorEventHandler OnError;
        public event WebSocketCloseEventHandler OnClose;

        private Uri uri;
        private Dictionary<string, string> headers;
        private List<string> subprotocols;
        private ClientWebSocket m_Socket = new ClientWebSocket();

        private CancellationTokenSource m_TokenSource;
        private CancellationToken m_CancellationToken;

        private readonly object OutgoingMessageLock = new object();
        private readonly object IncomingMessageLock = new object();

        private bool isSending = false;
        private List<ArraySegment<byte>> sendBytesQueue = new List<ArraySegment<byte>>();
        private List<ArraySegment<byte>> sendTextQueue = new List<ArraySegment<byte>>();

        public WebSocket(string url, Dictionary<string, string> headers = null)
        {
            this.uri = new Uri(url);

            if (headers == null)
            {
                this.headers = new Dictionary<string, string>();
            }
            else
            {
                this.headers = headers;
            }

            this.subprotocols = new List<string>();

            string protocol = this.uri.Scheme;
            if (!protocol.Equals("ws") && !protocol.Equals("wss"))
                throw new ArgumentException("Unsupported protocol: " + protocol);
        }

        public WebSocket(string url, string subprotocol, Dictionary<string, string> headers = null)
        {
            this.uri = new Uri(url);

            if (headers == null)
            {
                this.headers = new Dictionary<string, string>();
            }
            else
            {
                this.headers = headers;
            }

            this.subprotocols = new List<string> { subprotocol };

            string protocol = this.uri.Scheme;
            if (!protocol.Equals("ws") && !protocol.Equals("wss"))
                throw new ArgumentException("Unsupported protocol: " + protocol);
        }

        public WebSocket(string url, List<string> subprotocols, Dictionary<string, string> headers = null)
        {
            this.uri = new Uri(url);

            if (headers == null)
            {
                this.headers = new Dictionary<string, string>();
            }
            else
            {
                this.headers = headers;
            }

            this.subprotocols = subprotocols;

            string protocol = this.uri.Scheme;
            if (!protocol.Equals("ws") && !protocol.Equals("wss"))
                throw new ArgumentException("Unsupported protocol: " + protocol);
        }

        public void CancelConnection()
        {
            this.m_TokenSource?.Cancel();
        }

        public async Task Connect()
        {
            try
            {
                this.m_TokenSource = new CancellationTokenSource();
                this.m_CancellationToken = this.m_TokenSource.Token;

                this.m_Socket = new ClientWebSocket();

                foreach (var header in this.headers)
                {
                    this.m_Socket.Options.SetRequestHeader(header.Key, header.Value);
                }

                foreach (string subprotocol in this.subprotocols)
                {
                    this.m_Socket.Options.AddSubProtocol(subprotocol);
                }

                await this.m_Socket.ConnectAsync(this.uri, this.m_CancellationToken);
                OnOpen?.Invoke();

                await Receive();
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex.Message);
                OnClose?.Invoke(WebSocketCloseCode.Abnormal);
            }
            finally
            {
                if (this.m_Socket != null)
                {
                    this.m_TokenSource.Cancel();
                    this.m_Socket.Dispose();
                }
            }
        }

        public WebSocketState State
        {
            get
            {
                switch (this.m_Socket.State)
                {
                    case System.Net.WebSockets.WebSocketState.Connecting:
                        return WebSocketState.Connecting;

                    case System.Net.WebSockets.WebSocketState.Open:
                        return WebSocketState.Open;

                    case System.Net.WebSockets.WebSocketState.CloseSent:
                    case System.Net.WebSockets.WebSocketState.CloseReceived:
                        return WebSocketState.Closing;

                    case System.Net.WebSockets.WebSocketState.Closed:
                        return WebSocketState.Closed;

                    default:
                        return WebSocketState.Closed;
                }
            }
        }

        public Task Send(byte[] bytes)
        {
            // return m_Socket.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
            return SendMessage(this.sendBytesQueue, WebSocketMessageType.Binary, new ArraySegment<byte>(bytes));
        }

        public Task SendText(string message)
        {
            var encoded = Encoding.UTF8.GetBytes(message);

            // m_Socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            return SendMessage(this.sendTextQueue, WebSocketMessageType.Text, new ArraySegment<byte>(encoded, 0, encoded.Length));
        }

        private async Task SendMessage(List<ArraySegment<byte>> queue, WebSocketMessageType messageType, ArraySegment<byte> buffer)
        {
            // Return control to the calling method immediately.
            // await Task.Yield ();

            // Make sure we have data.
            if (buffer.Count == 0)
            {
                return;
            }

            // The state of the connection is contained in the context Items dictionary.
            bool sending;

            lock (this.OutgoingMessageLock)
            {
                sending = this.isSending;

                // If not, we are now.
                if (!this.isSending)
                {
                    this.isSending = true;
                }
            }

            if (!sending)
            {
                // Lock with a timeout, just in case.
                if (!Monitor.TryEnter(this.m_Socket, 1000))
                {
                    // If we couldn't obtain exclusive access to the socket in one second, something is wrong.
                    await this.m_Socket.CloseAsync(WebSocketCloseStatus.InternalServerError, string.Empty, this.m_CancellationToken);
                    return;
                }

                try
                {
                    // Send the message synchronously.
                    var t = this.m_Socket.SendAsync(buffer, messageType, true, this.m_CancellationToken);
                    t.Wait(this.m_CancellationToken);
                }
                finally
                {
                    Monitor.Exit(this.m_Socket);
                }

                // Note that we've finished sending.
                lock (this.OutgoingMessageLock)
                {
                    this.isSending = false;
                }

                // Handle any queued messages.
                await HandleQueue(queue, messageType);
            }
            else
            {
                // Add the message to the queue.
                lock (this.OutgoingMessageLock)
                {
                    queue.Add(buffer);
                }
            }
        }

        private async Task HandleQueue(List<ArraySegment<byte>> queue, WebSocketMessageType messageType)
        {
            var buffer = new ArraySegment<byte>();
            lock (this.OutgoingMessageLock)
            {
                // Check for an item in the queue.
                if (queue.Count > 0)
                {
                    // Pull it off the top.
                    buffer = queue[0];
                    queue.RemoveAt(0);
                }
            }

            // Send that message.
            if (buffer.Count > 0)
            {
                await SendMessage(queue, messageType, buffer);
            }
        }

        private List<byte[]> m_MessageList = new List<byte[]>();
        private List<string> m_TextMessageList = new List<string>();

        // simple dispatcher for queued messages.
        public void DispatchMessageQueue()
        {
            if (this.m_MessageList.Count == 0 && this.m_TextMessageList.Count == 0)
            {
                return;
            }

            // Binary messages
            List<byte[]> messageListCopy = null;

            // Text messages
            List<string> textMessageListCopy = null;

            lock (this.IncomingMessageLock)
            {

                // Copy binary messages
                if (this.m_MessageList.Count != 0)
                {
                    messageListCopy = new List<byte[]>(this.m_MessageList);
                    this.m_MessageList.Clear();
                }

                // Copy text messages
                if (this.m_TextMessageList.Count != 0)
                {
                    textMessageListCopy = new List<string>(this.m_TextMessageList);
                    this.m_TextMessageList.Clear();
                }
            }

            // Broadcast binary messages
            if (messageListCopy != null)
            {
                var len = messageListCopy.Count;
                for (int i = 0; i < len; i++)
                {
                    OnMessage?.Invoke(messageListCopy[i]);
                }
            }

            // Broadcast text messages
            if (textMessageListCopy != null)
            {
                var len = textMessageListCopy.Count;
                for (int i = 0; i < len; i++)
                {
                    OnTextMessage?.Invoke(textMessageListCopy[i]);
                }
            }
        }

        public async Task Receive()
        {
            WebSocketCloseCode closeCode = WebSocketCloseCode.Abnormal;
            await new WaitForBackgroundThread();

            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[8192]);
            try
            {
                while (this.m_Socket.State == System.Net.WebSockets.WebSocketState.Open)
                {
                    WebSocketReceiveResult result = null;

                    using (var ms = new MemoryStream())
                    {
                        do
                        {
                            result = await this.m_Socket.ReceiveAsync(buffer, this.m_CancellationToken);
                            ms.Write(buffer.Array, buffer.Offset, result.Count);
                        }
                        while (!result.EndOfMessage);

                        ms.Seek(0, SeekOrigin.Begin);

                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            lock (this.IncomingMessageLock)
                            {
                                this.m_TextMessageList.Add(Encoding.UTF8.GetString(ms.ToArray()));
                            }

                            //using (var reader = new StreamReader(ms, Encoding.UTF8))
                            //{
                            //	string message = reader.ReadToEnd();
                            //	OnMessage?.Invoke(this, new MessageEventArgs(message));
                            //}
                        }
                        else if (result.MessageType == WebSocketMessageType.Binary)
                        {
                            lock (this.IncomingMessageLock)
                            {
                                this.m_MessageList.Add(ms.ToArray());
                            }
                        }
                        else if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await Close();
                            closeCode = WebSocketHelpers.ParseCloseCodeEnum((int)result.CloseStatus);
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                this.m_TokenSource.Cancel();
            }
            finally
            {
                await new WaitForUpdate();
                OnClose?.Invoke(closeCode);
            }
        }

        public async Task Close()
        {
            if (State == WebSocketState.Open)
            {
                await this.m_Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, this.m_CancellationToken);
            }
        }
    }
#endif

    ///
    /// Factory
    ///

    /// <summary>
    /// Class providing static access methods to work with JSLIB WebSocket or WebSocketSharp interface
    /// </summary>
    public static class WebSocketFactory
    {

#if UNITY_WEBGL && !UNITY_EDITOR
        /* Map of websocket instances */
        public static Dictionary<Int32, WebSocket> instances = new Dictionary<Int32, WebSocket>();

        /* Delegates */
        public delegate void OnOpenCallback(int instanceId);
        public delegate void OnMessageCallback(int instanceId, System.IntPtr msgPtr, int msgSize);
        public delegate void OnTextMessageCallback(int instanceId, System.IntPtr msgPtr);
        public delegate void OnErrorCallback(int instanceId, System.IntPtr errorPtr);
        public delegate void OnCloseCallback(int instanceId, int closeCode);

        /* WebSocket JSLIB callback setters and other functions */
        [DllImport("__Internal")]
        public static extern int WebSocketAllocate(string url);

        [DllImport("__Internal")]
        public static extern int WebSocketAddSubProtocol(int instanceId, string subprotocol);

        [DllImport("__Internal")]
        public static extern void WebSocketFree(int instanceId);

        [DllImport("__Internal")]
        public static extern void WebSocketSetOnOpen(OnOpenCallback callback);

        [DllImport("__Internal")]
        public static extern void WebSocketSetOnMessage(OnMessageCallback callback);

        [DllImport("__Internal")]
        public static extern void WebSocketSetOnTextMessage(OnTextMessageCallback callback);

        [DllImport("__Internal")]
        public static extern void WebSocketSetOnError(OnErrorCallback callback);

        [DllImport("__Internal")]
        public static extern void WebSocketSetOnClose(OnCloseCallback callback);

        /* If callbacks was initialized and set */
        public static bool isInitialized = false;

        /*
         * Initialize WebSocket callbacks to JSLIB
         */
        public static void Initialize()
        {

            WebSocketSetOnOpen(DelegateOnOpenEvent);
            WebSocketSetOnMessage(DelegateOnMessageEvent);
            WebSocketSetOnTextMessage(DelegateOnTextMessageEvent);
            WebSocketSetOnError(DelegateOnErrorEvent);
            WebSocketSetOnClose(DelegateOnCloseEvent);

            isInitialized = true;

        }

        /// <summary>
        /// Called when instance is destroyed (by destructor)
        /// Method removes instance from map and free it in JSLIB implementation
        /// </summary>
        /// <param name="instanceId">Instance identifier.</param>
        public static void HandleInstanceDestroy(int instanceId)
        {

            instances.Remove(instanceId);
            WebSocketFree(instanceId);

        }

        [MonoPInvokeCallback(typeof(OnOpenCallback))]
        public static void DelegateOnOpenEvent(int instanceId)
        {

            WebSocket instanceRef;

            if (instances.TryGetValue(instanceId, out instanceRef))
            {
                instanceRef.DelegateOnOpenEvent();
            }

        }

        [MonoPInvokeCallback(typeof(OnMessageCallback))]
        public static void DelegateOnMessageEvent(int instanceId, System.IntPtr msgPtr, int msgSize)
        {

            WebSocket instanceRef;

            if (instances.TryGetValue(instanceId, out instanceRef))
            {
                byte[] msg = new byte[msgSize];
                Marshal.Copy(msgPtr, msg, 0, msgSize);

                instanceRef.DelegateOnMessageEvent(msg);
            }

        }

        [MonoPInvokeCallback(typeof(OnTextMessageCallback))]
        public static void DelegateOnTextMessageEvent(int instanceId, System.IntPtr msgPtr)
        {

            WebSocket instanceRef;

            if (instances.TryGetValue(instanceId, out instanceRef))
            {
                var msg = Marshal.PtrToStringUTF8(msgPtr);
                instanceRef.DelegateOnTextMessageEvent(msg);
            }

        }

        [MonoPInvokeCallback(typeof(OnErrorCallback))]
        public static void DelegateOnErrorEvent(int instanceId, System.IntPtr errorPtr)
        {

            WebSocket instanceRef;

            if (instances.TryGetValue(instanceId, out instanceRef))
            {

                string errorMsg = Marshal.PtrToStringAuto(errorPtr);
                instanceRef.DelegateOnErrorEvent(errorMsg);

            }

        }

        [MonoPInvokeCallback(typeof(OnCloseCallback))]
        public static void DelegateOnCloseEvent(int instanceId, int closeCode)
        {

            WebSocket instanceRef;

            if (instances.TryGetValue(instanceId, out instanceRef))
            {
                instanceRef.DelegateOnCloseEvent(closeCode);
            }

        }
#endif

        /// <summary>
        /// Create WebSocket client instance
        /// </summary>
        /// <returns>The WebSocket instance.</returns>
        /// <param name="url">WebSocket valid URL.</param>
        public static WebSocket CreateInstance(string url)
        {
            return new WebSocket(url);
        }

    }

}
