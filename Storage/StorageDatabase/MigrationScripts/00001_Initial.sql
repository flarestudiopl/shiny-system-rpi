CREATE TABLE IF NOT EXISTS `User` (
    `UserId`            INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    `Login`             TEXT    NOT NULL,
    `PasswordHash`      TEXT    NOT NULL,
    `LastSeenIpAddress` TEXT,
    `LastLogonDateTime` INTEGER NOT NULL,
    `IsActiveBool`      INTEGER NOT NULL DEFAULT 1,
    `IsBrowseableBool`  INTEGER NOT NULL DEFAULT 1,
    `CreatedBy`         INTEGER NOT NULL,
    `CreatedDateTime`   INTEGER NOT NULL,
    `DisabledBy`        INTEGER,
    `DisabledDateTime`  INTEGER
);

CREATE UNIQUE INDEX IF NOT EXISTS `User_Login_IsActiveBool` ON `User` (
    `Login`    ASC,
    `IsActiveBool`    ASC
) WHERE [IsActiveBool] = 1;

INSERT INTO `User` 
SELECT (SELECT MAX(UserId) + 1 FROM `User`),'user','3da810408ed48e255a05f80798db255a4ae32b205e895f08ffe0833338d03d71','::1',636727249674036253,1,1,-1,636626164845313036,NULL,NULL
WHERE NOT EXISTS (SELECT 1 FROM `User` WHERE `Login` = 'user');

CREATE TABLE IF NOT EXISTS `Counter` (
    `CounterId`      INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    `HeaterId`       INTEGER NOT NULL,
    `CountedSeconds` INTEGER NOT NULL,
    `StartDateTime`  INTEGER NOT NULL,
    `ResetDateTime`  INTEGER,
    `ResettedBy`     INTEGER
);
