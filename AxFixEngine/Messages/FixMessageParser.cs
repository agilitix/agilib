﻿using AxFixEngine.Dialects;
using QuickFix;
using QuickFix.DataDictionary;
using QuickFix.Fields;
using Message = QuickFix.Message;

namespace AxFixEngine.Messages
{
    public class FixMessageParser : IFixMessageParser
    {
        private readonly IFixDialects _dialects;
        private readonly IMessageFactory _factory;

        public FixMessageParser(IFixDialects dialects, IMessageFactory factory)
        {
            _dialects = dialects;
            _factory = factory;
        }

        public Message ParseMessage(string fixMessage, bool validate = true)
        {
            SessionID sessionId = ParseSessionID(fixMessage);
            MsgType msgType = Message.IdentifyType(fixMessage);
            DataDictionary dataDictionary = _dialects.GetDataDictionary(sessionId);

            Message message = _factory.Create(sessionId.BeginString, msgType.getValue());

            message.FromString(fixMessage,
                               validate,
                               dataDictionary,
                               dataDictionary,
                               null);

            return message;
        }

        public BeginString ParseBeginString(string fixMessage)
        {
            return new BeginString(Message.ExtractBeginString(fixMessage));
        }

        public MsgType ParseMsgType(string fixMessage)
        {
            return Message.IdentifyType(fixMessage);
        }

        public SessionID ParseSessionID(string fixMessage)
        {
            Message msg = new Message();
            return msg.FromStringHeader(fixMessage)
                       ? msg.GetSessionID(msg)
                       : null;
        }

        public Header ParseHeader(string fixMessage)
        {
            Message msg = new Message();
            return msg.FromStringHeader(fixMessage)
                       ? msg.Header
                       : null;
        }
    }
}
