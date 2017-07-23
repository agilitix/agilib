﻿using System;
using System.Collections.Generic;
using AxFixEngine.Extensions;
using AxFixEngine.Interfaces;
using QuickFix;
using QuickFix.DataDictionary;

namespace AxFixEngine.Dialects
{
    public class FixDialects : IFixDialects
    {
        protected readonly IDictionary<SessionID, DataDictionary> _dataDictionaries = new Dictionary<SessionID, DataDictionary>();
        protected readonly IDictionary<string, DataDictionary> _dataDictionariesBySpecFile = new Dictionary<string, DataDictionary>();

        public FixDialects(string fixSettingsFile)
            : this(new SessionSettings(fixSettingsFile))
        {
        }

        public FixDialects(SessionSettings fixSettings)
        {
            HashSet<SessionID> sessions = fixSettings.GetSessions();
            foreach (SessionID sessionId in sessions)
            {
                Dictionary settings = fixSettings.Get(sessionId);
                string specFile = settings.GetString(SessionSettings.DATA_DICTIONARY);
                DataDictionary dataDictionary;
                if (_dataDictionariesBySpecFile.TryGetValue(specFile, out dataDictionary))
                {
                    _dataDictionaries[sessionId] = _dataDictionaries[sessionId.ToReverseSessionID()] = dataDictionary;
                }
                else
                {
                    dataDictionary = new DataDictionary();
                    dataDictionary.Load(specFile);
                    _dataDictionaries[sessionId] = _dataDictionaries[sessionId.ToReverseSessionID()] = _dataDictionariesBySpecFile[specFile] = dataDictionary;
                }
            }
        }

        public IDictionary<SessionID, DataDictionary> GetAllDataDictionaries()
        {
            return _dataDictionaries;
        }

        public DataDictionary GetDataDictionary(SessionID sessionId)
        {
            DataDictionary dataDictionary;
            if (TryGetDataDictionary(sessionId, out dataDictionary))
            {
                return dataDictionary;
            }

            throw new ArgumentOutOfRangeException();
        }

        public bool TryGetDataDictionary(SessionID sessionId, out DataDictionary dataDictionary)
        {
            return _dataDictionaries.TryGetValue(sessionId, out dataDictionary);
        }
    }
}
