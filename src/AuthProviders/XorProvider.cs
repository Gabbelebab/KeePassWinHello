﻿using System;
using System.Linq;
using System.Windows.Forms;

namespace KeePassWinHello
{
    class XorProvider : IAuthProvider
    {
        private const byte _entropy = 42;

        private readonly UIContextManager _uiContextManager;

        public XorProvider(AuthCacheType authCacheType, UIContextManager uiContextManager)
        {
            CurrentCacheType = authCacheType;
            _uiContextManager = uiContextManager;
        }

        public AuthCacheType CurrentCacheType { get; private set; } // TDB

        public void ClaimCurrentCacheType(AuthCacheType newType)
        {
            CurrentCacheType = newType;

            if (newType == AuthCacheType.Persistent)
            {
                string message = "Default message for persistent auth type";
                var uiContext = _uiContextManager.CurrentContext;
                if (uiContext != null && !String.IsNullOrEmpty(uiContext.Message))
                    message = uiContext.Message;

                var dlgRslt = MessageBox.Show(uiContext, message, "Test cache type change", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dlgRslt != DialogResult.OK)
                    throw new AuthProviderUserCancelledException();
            }
            else
            {
                MessageBox.Show(_uiContextManager.CurrentContext, "Switched to local.", "Keys removed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public byte[] Encrypt(byte[] data)
        {
            return data.Select(x => (byte)(x ^ _entropy)).ToArray();
        }

        public byte[] PromptToDecrypt(byte[] data)
        {
            string message = "Default message for encrypt";
            var uiContext = _uiContextManager.CurrentContext;
            if (uiContext != null && !String.IsNullOrEmpty(uiContext.Message))
                message = uiContext.Message;

            var dlgRslt = MessageBox.Show(uiContext, message, "Windows Security", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dlgRslt == DialogResult.OK)
            {
                return Encrypt(data);
            }
            else
            {
                throw new AuthProviderUserCancelledException();
            }
        }
    }
}
