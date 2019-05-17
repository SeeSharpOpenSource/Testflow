﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using Testflow.Modules;
using Testflow.Runtime.Data;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;

namespace Testflow.DataMaintainer
{
    internal class RuntimeDatabaseProxy : DatabaseProxy
    {
        public RuntimeDatabaseProxy(IModuleConfigData configData) : base(configData, true)
        {
        }

        public override int GetTestInstanceCount(string fileterString)
        {
            throw new System.InvalidProgramException();
        }

        public override TestInstanceData GetTestInstanceData(string runtimeHash)
        {
            throw new System.InvalidProgramException();
        }

        public override IList<TestInstanceData> GetTestInstanceDatas(string filterString)
        {
            throw new System.InvalidProgramException();
        }
        
        public override void DeleteTestInstance(string fileterString)
        {
            throw new System.InvalidProgramException();
        }

        public override IList<SessionResultData> GetSessionResults(string runtimeHash)
        {
            throw new System.InvalidProgramException();
        }

        public override SessionResultData GetSessionResult(string runtimeHash, int sessionId)
        {
            throw new System.InvalidProgramException();
        }

        public override IList<SequenceResultData> GetSequenceResultDatas(string runtimeHash, int sessionId)
        {
            throw new System.InvalidProgramException();
        }

        public override SequenceResultData GetSequenceResultData(string runtimeHash, int sessionId, int sequenceIndex)
        {
            throw new System.InvalidProgramException();
        }
    }
}