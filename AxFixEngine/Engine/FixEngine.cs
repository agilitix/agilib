﻿using System.Collections.Generic;
using AxCommonLogger;
using AxCommonLogger.Interfaces;
using AxFixEngine.Dialects;
using AxFixEngine.Factories;
using AxFixEngine.Handlers;
using AxFixEngine.Interfaces;
using QuickFix;

namespace AxFixEngine.Engine
{
    public class FixEngine : IFixEngine
    {
        protected ILoggerFacade Logger = LoggerFacadeProvider.GetDeclaringTypeLogger();
        protected IList<SessionID> _sessions;
        protected IFixConnectorFactory _connectorFactory;
        protected IFixConnector _acceptor;
        protected IFixConnector _initiator;
        protected SessionSettings _acceptorSettings;
        protected SessionSettings _initiatorSettings;

        public IList<SessionID> Sessions => _sessions;

        public FixEngine(string acceptorConfig, string initiatorConfig)
        {
            FixDialectsProvider.Attach(new FixDialects());

            _connectorFactory = new FixConnectorFactory();
            _sessions = new List<SessionID>();

            if (!string.IsNullOrWhiteSpace(acceptorConfig))
            {
                _acceptorSettings = new SessionSettings(acceptorConfig);
                if (_acceptorSettings.Get().GetString("ConnectionType") != "acceptor")
                {
                    throw new ConfigError("The config file is not valid for an acceptor");
                }

                foreach (SessionID sessionId in _acceptorSettings.GetSessions())
                {
                    _sessions.Add(sessionId);
                }
            }

            if (!string.IsNullOrWhiteSpace(initiatorConfig))
            {
                _initiatorSettings = new SessionSettings(initiatorConfig);
                if (_initiatorSettings.Get().GetString("ConnectionType") != "initiator")
                {
                    throw new ConfigError("The config file is not valid for an initiator");
                }

                foreach (SessionID sessionId in _initiatorSettings.GetSessions())
                {
                    _sessions.Add(sessionId);
                }
            }
        }

        public void InitializeFixApplication(IFixMessageHandlerProvider messageHandlerProvider)
        {
            if (_acceptorSettings != null)
            {
                FixDialectsProvider.Dialects.AddDataDictionaries(_acceptorSettings);
                IApplication fixApp = new FixApplication(messageHandlerProvider);
                _acceptor = _connectorFactory.CreateAcceptor(fixApp, _acceptorSettings);
            }
            if (_initiatorSettings != null)
            {
                FixDialectsProvider.Dialects.AddDataDictionaries(_initiatorSettings);
                IApplication fixApp = new FixApplication(messageHandlerProvider);
                _initiator = _connectorFactory.CreateInitiator(fixApp, _initiatorSettings);
            }
        }

        public void Start()
        {
            _acceptor?.Start();
            _initiator?.Start();
        }

        public void Stop()
        {
            _initiator?.Stop();
            _acceptor?.Stop();
        }
    }
}
