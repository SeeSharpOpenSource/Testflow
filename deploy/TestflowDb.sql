-- TestflowData数据库的表

-- 运行实例表
CREATE TABLE Testflow_TestInstances(
	RuntimeHash TEXT PRIMARY KEY NOT NULL,
	Name TEXT NOT NULL,
	Description TEXT,
	TestProjectName TEXT NOT NULL,
	TestProjectDescription TEXT,
	StartGenTime TEXT NOT NULL,
	EndGenTime TEXT NOT NULL,
	StartTime TEXT NOT NULL,
	EndTime TEXT NOT NULL,
	ElapsedTime REAL NOT NULL
);

-- 会话结果表
CREATE TABLE Testflow_SessionResults(
	RuntimeHash TEXT NOT NULL,
	Name TEXT NOT NULL,
	Description TEXT,
	SessionId INTEGER NOT NULL,
	SequenceHash TEXT NOT NULL,
	StartTime TEXT NOT NULL,
	EndTime TEXT NOT NULL,
	ElapsedTime TEXT NOT NULL,
	SessionState TEXT NOT NULL,
	FailedInfo TEXT,
	PRIMARY KEY (RuntimeHash,SessionId),
	FOREIGN KEY (RuntimeHash) REFERENCES Testflow_TestInstances(RuntimeHash)
);

-- 序列结果表
CREATE TABLE Testflow_SequenceResults(
	RuntimeHash TEXT NOT NULL,
	Name TEXT NOT NULL,
	Description TEXT,
	SessionId INTEGER NOT NULL,
	SequenceIndex INTEGER NOT NULL,
	SequenceResult  TEXT NOT NULL,
	StartTime TEXT NOT NULL,
	EndTime TEXT NOT NULL,
	ElapsedTime REAL NOT NULL,
	FailInfo TEXT,
	FailStack TEXT NOT NULL,
	PRIMARY KEY (RuntimeHash,SessionId,SequenceIndex),
	FOREIGN KEY (RuntimeHash,SessionId) REFERENCES Testflow_SessionResults(RuntimeHash,SessionId)
);

-- 运行时状态表
CREATE TABLE Testflow_RuntimeStatusDatas(
	RuntimeHash TEXT NOT NULL,
	SessionId INTEGER NOT NULL,
	SequenceIndex INTEGER NOT NULL,
	StatusIndex INTEGER NOT NULL,
	RecordTime TEXT NOT NULL,
	ElapsedTime REAL NOT NULL,
	StepResult TEXT NOT NULL,
	Stack TEXT NOT NULL,
	WatchData TEXT,
	PRIMARY KEY (RuntimeHash,StatusIndex),
	FOREIGN KEY (RuntimeHash) REFERENCES Testflow_TestInstances(RuntimeHash)
);

-- 性能数据表
CREATE TABLE Testflow_PerformanceDatas(
	RuntimeHash TEXT NOT NULL,
	SessionId INTEGER NOT NULL,
	StatusIndex INTEGER NOT NULL,
	RecordTime TEXT NOT NULL,
	MemoryUsed INTEGER NOT NULL,
	MemoryAllocated INTEGER NOT NULL,
	ProcessorTime REAL NOT NULL,
	PRIMARY KEY (RuntimeHash,StatusIndex),
	FOREIGN KEY (RuntimeHash) REFERENCES Testflow_TestInstances(RuntimeHash)
);