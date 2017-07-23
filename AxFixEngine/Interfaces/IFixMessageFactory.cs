﻿using QuickFix;
using QuickFix.Fields;

namespace AxFixEngine.Interfaces
{
    public interface IFixMessageFactory
    {
        T CreateMessage<T>(SessionID sessionID, string msgType) where T : Message;

        Message CreateMessageFromString(string fixMessage);
        Message CreateValidatedMessageFromString(string fixMessage);
    }
}