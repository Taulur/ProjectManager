CREATE TABLE Projects (
id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
name NVARCHAR(MAX),
description NVARCHAR(MAX),
created_at datetime,
updated_at datetime,
);

CREATE TABLE ProjectUsers (
id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
user_id int,
project_id int,
role_id int,
updated_at datetime,
);

CREATE TABLE Roles (
id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
title NVARCHAR(MAX),
);

CREATE TABLE Users (
id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
username NVARCHAR(MAX),
fullname NVARCHAR(MAX),
password NVARCHAR(MAX),
created_at datetime,
systemrole_id int,
);

CREATE TABLE SystemRoles (
id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
title NVARCHAR(MAX),
);

CREATE TABLE Tasks (
id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
title NVARCHAR(MAX),
description NVARCHAR(MAX),
due_date datetime,
created_at datetime,
updated_at datetime,
project_id int,
status_id int,
priority_id int,
created_by int,
assigned_to int,
);

CREATE TABLE Statuses (
id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
title NVARCHAR(MAX),
);

CREATE TABLE Priorities (
id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
title NVARCHAR(MAX),
);

CREATE TABLE Comments (
id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
task_id int,
user_id int,
text NVARCHAR(MAX),
created_at datetime,
);

CREATE TABLE History (
id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
action_id int,
user_id int,
target_id int,
created_at datetime,
);

CREATE TABLE Targets (
id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
title NVARCHAR(MAX),
);

CREATE TABLE Actions (
id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
title NVARCHAR(MAX),
);