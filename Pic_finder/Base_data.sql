Use AniPic_service_data;
Go

CREATE TABLE Users(
Id BigInt PRIMARY KEY, 
IsBot Bit, 
FirstName NVarChar(256), 
LastName NVarChar(256), 
Username VarChar(50), 
LanguageCode VarChar(10),
Serialize NText
);

CREATE TABLE Files(
MessageId BigInt,
FileId VarChar(30) PRIMARY KEY,
FileSize VarChar(10),
MimeType VarChar(15)
);

CREATE TABLE Chat(
Id BigInt PRIMARY KEY,
ChatType smallint,
Title NVarChar(255),
Username VarChar(255),
ChatDescription ntext,
InviteLink VarChar(255),
Serialized NText
);

CREATE TABLE TMessages(
MsgId BigInt,
ChatId BigInt,
Id BigInt Primary key identity(0, 1),
Caption NVarChar(255),
Document VarChar(30),
FromUser BigInt,
DateSent DateTime,
IsForwarded bit,
ForwardFrom BigInt,
ForwardFromChat BigInt,
ForwardSignature VarChar(255),
MsgText NText,
ReplyToMessage BigInt,
MessageType tinyint,
Serialized nText
);

Alter table TMessages
add constraint FK_TMessages_Chat foreign key (ChatId)
	references Chat (Id)
	on delete cascade
	on update cascade
;

Alter table TMessages
add constraint FK_TMessages_User foreign key (FromUser)
	references Users (Id)
	on delete cascade
	on update cascade
;

use AniPic_service_data;
go

/*
create trigger ChatIntegrate on Chat instead of insert as
if exists(select Id from inserted intersect select Id from Chat)
begin
update ChatA
Set 
	ChatA.ChatType=ChatB.ChatType,
	ChatA.Title=ChatB.Title,
	ChatA.Username=ChatB.Username,
	ChatA.ChatDescription=ChatB.ChatDescription,
	ChatA.InviteLink=ChatB.InviteLink,
	ChatA.Serialized=ChatB.Serialized
from Chat as ChatA inner join inserted as ChatB on ChatA.Id=ChatB.Id
where ChatA.Id in (
	select Id from ChatA
	intersect
	select Id from ChatB
	)
end
if exists(select Id from inserted except select Id from Chat)
begin 
insert into Chat select * from (select * from inserted where inserted.Id not in 
(select Id from inserted intersect select Id from Chat)) as NMsgs;
end
*/

Alter table Files
add constraint FK_Files_TMessages foreign key (MessageId)
	references TMessages (Id)
	on delete no action
	on update no action
;
