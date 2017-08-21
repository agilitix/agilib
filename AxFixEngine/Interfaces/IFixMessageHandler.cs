﻿using QuickFix;

namespace AxFixEngine.Interfaces
{
    public interface IFixMessageHandler
    {
        void OnCreate(SessionID sessionId);
        void OnLogon(SessionID sessionId);
        void OnLogout(SessionID sessionId);

        void OnAdmin(Message message, SessionID sessionId);
        void OnApp(Message message, SessionID sessionId);
    }
}
