﻿namespace Winook
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    public class MouseHook : HookBase
    {
        #region Fields

        private const int HookMessageSizeInBytes = 24;
        private const HookType MouseHookType = HookType.Mouse; // WH_MOUSE

        private Dictionary<int, MouseEventHandler> _messageHandlers = new Dictionary<int, MouseEventHandler>();

        #endregion

        #region Events

        public event EventHandler<MouseMessageEventArgs> MessageReceived;
        public event EventHandler<MouseMessageEventArgs> MouseMove;
        public event EventHandler<MouseMessageEventArgs> LeftButtonDown;
        public event EventHandler<MouseMessageEventArgs> LeftButtonUp;
        public event EventHandler<MouseMessageEventArgs> LeftButtonDblClk;
        public event EventHandler<MouseMessageEventArgs> RightButtonDown;
        public event EventHandler<MouseMessageEventArgs> RightButtonUp;
        public event EventHandler<MouseMessageEventArgs> RightButtonDblClk;
        public event EventHandler<MouseMessageEventArgs> MiddleButtonDown;
        public event EventHandler<MouseMessageEventArgs> MiddleButtonUp;
        public event EventHandler<MouseMessageEventArgs> MiddleButtonDblClk;
        public event EventHandler<MouseMessageEventArgs> MouseWheel;
        public event EventHandler<MouseMessageEventArgs> XButtonDown;
        public event EventHandler<MouseMessageEventArgs> XButtonUp;
        public event EventHandler<MouseMessageEventArgs> XButtonDblClk;
        public event EventHandler<MouseMessageEventArgs> MouseHWheel;

        public delegate void MouseEventHandler(object sender, MouseMessageEventArgs e);

        #endregion

        #region Constructors

        public MouseHook(int processId)
            : base(processId, MouseHookType, HookMessageSizeInBytes)
        {
        }

        #endregion

        #region Methods

        public void AddHandler(MouseMessageCode mouseMessageCode, MouseEventHandler handler)
            => AddHandler((int)mouseMessageCode, handler);

        public void AddHandler(int mouseMessageCode, MouseEventHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (!_messageHandlers.ContainsKey(mouseMessageCode))
            {
                _messageHandlers.Add(mouseMessageCode, null);
            }

            _messageHandlers[mouseMessageCode] += handler;
        }

        public void RemoveHandler(MouseMessageCode mouseMessageCode, MouseEventHandler handler)
            => RemoveHandler((int)mouseMessageCode, handler);

        public void RemoveHandler(int mouseMessageCode, MouseEventHandler handler)
        {
            if (_messageHandlers.ContainsKey(mouseMessageCode))
            {
                _messageHandlers[mouseMessageCode] -= handler ?? throw new ArgumentNullException(nameof(handler));
            }
        }

        protected override void OnMessageReceived(object sender, MessageEventArgs e)
        {
            Contract.Requires(e != null);

            var eventArgs = new MouseMessageEventArgs
            {
                MessageCode = BitConverter.ToInt32(e.Bytes, 0),
                X = BitConverter.ToInt32(e.Bytes, 4),
                Y = BitConverter.ToInt32(e.Bytes, 8),
                Handle = BitConverter.ToInt32(e.Bytes, 12),
                HitTestCode = BitConverter.ToInt32(e.Bytes, 16),
                Delta = BitConverter.ToInt16(e.Bytes, 20),
            };

            Debug.WriteLine($"Code: {eventArgs.MessageCode}; X: {eventArgs.X}; Y: {eventArgs.Y}; Delta: {eventArgs.Delta}");

            MessageReceived?.Invoke(this, eventArgs);

            switch (GetMessageCode(eventArgs.MessageCode))
            {
                case MouseMessageCode.MouseMove:
                    MouseMove?.Invoke(this, eventArgs);
                    break;
                case MouseMessageCode.LeftButtonDown:
                    LeftButtonDown?.Invoke(this, eventArgs);
                    break;
                case MouseMessageCode.LeftButtonUp:
                    LeftButtonUp?.Invoke(this, eventArgs);
                    break;
                case MouseMessageCode.LeftButtonDblClk:
                    LeftButtonDblClk?.Invoke(this, eventArgs);
                    break;
                case MouseMessageCode.RightButtonDown:
                    RightButtonDown?.Invoke(this, eventArgs);
                    break;
                case MouseMessageCode.RightButtonUp:
                    RightButtonUp?.Invoke(this, eventArgs);
                    break;
                case MouseMessageCode.RightButtonDblClk:
                    RightButtonDblClk?.Invoke(this, eventArgs);
                    break;
                case MouseMessageCode.MiddleButtonDown:
                    MiddleButtonDown?.Invoke(this, eventArgs);
                    break;
                case MouseMessageCode.MiddleButtonUp:
                    MiddleButtonUp?.Invoke(this, eventArgs);
                    break;
                case MouseMessageCode.MiddleButtonDblClk:
                    MiddleButtonDblClk?.Invoke(this, eventArgs);
                    break;
                case MouseMessageCode.MouseWheel:
                    MouseWheel?.Invoke(this, eventArgs);
                    break;
                case MouseMessageCode.XButtonDown:
                    XButtonDown?.Invoke(this, eventArgs);
                    break;
                case MouseMessageCode.XButtonUp:
                    XButtonUp?.Invoke(this, eventArgs);
                    break;
                case MouseMessageCode.XButtonDblClk:
                    XButtonDblClk?.Invoke(this, eventArgs);
                    break;
                case MouseMessageCode.MouseHWheel:
                    MouseHWheel?.Invoke(this, eventArgs);
                    break;
                default:
                    break;
            }

            if (_messageHandlers.ContainsKey(eventArgs.MessageCode))
            {
                _messageHandlers[eventArgs.MessageCode]?.Invoke(this, eventArgs);
            }
        }

        private MouseMessageCode? GetMessageCode(int messageCode)
        {
            if (Enum.IsDefined(typeof(MouseMessageCode), messageCode))
            {
                return (MouseMessageCode)messageCode;
            }

            return null;
        }

        #endregion
    }
}
