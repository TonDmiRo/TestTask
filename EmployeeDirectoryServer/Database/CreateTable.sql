﻿CREATE DATABASE EmployeeDirectory
GO
USE EmployeeDirectory
go
create table Employees (
ID int NOT NULL IDENTITY,
LastName nvarchar(30), 
FirstName nvarchar(30),
MiddleName nvarchar(30),
Birthday date
CONSTRAINT PK_Employees PRIMARY KEY(ID) 
)

go 
INSERT Employees(LastName,FirstName, MiddleName, Birthday) VALUES
(N'Белозёров',N'Аввакуум', N'Олегович','19930219'),
(N'Беляев',N'Денис', N'Александрович','19930219'),
(N'Бобров',N'Станислав', N'Дмитрьевич','19670219'),
(N'Мартынов',N'Евгений', N'Эльдарович','19670219'),
(N'Зыков',N'Мирослав', N'Оскарович','19670219'),
(N'Александров',N'Варлаам', N'Германович','19670219'),
(N'Красильников',N'Ян', N'Пантелеймонович','19670219'),
(N'Морозов',N'Афанасий', N'Кириллович','19880219'),
(N'Назаров',N'Виталий', N'Кимович','19320219'),
(N'Белоусов',N'Осип', N'Проклович','19980219'),
(N'Лыткин',N'Ипполит', N'Георгьевич','19960219'),
(N'Никитин',N'Гордей', N'Дмитрьевич','19450219'),
(N'Крылов',N'Осип', N'Михайлович','19650219'),
(N'Федотов',N'Арсений', N'Данилович','19780219'),
(N'Туров',N'Павел', N'Валентинович','19990219'),
(N'Блохин',N'Валерий', N'Олегович','19760219'),
(N'Федосеев',N'Евгений', N'Антонинович','19440219'),
(N'Стрелков',N'Кондратий', N'Ефимович','19320219'),
(N'Сорокин',N'Емельян', N'Давидович','19980219'),
(N'Карпов',N'Евдоким', N'Макарович','19980219'),
(N'Фёдорова',N'Данна', N'Георгиевна','19980219'),
(N'Одинцова',N'Аурелия', N'Анатольевна','19980219'),
(N'Андреева', N'Аида', N'Никитевна','19980219'),
(N'Шашкова',N'Доля', N'Мартыновна','19980219'),
(N'Кузнецова',N'Веселина', N'Геннадьевна','19980219'),
(N'Воробьёва',N'Инга', N'Вячеславовна','19980219'),
(N'Молчанова',N'Элизабет', N'Всеволодовна','19980219'),
(N'Сазонова',N'Моника', N'Владимировна','19980219'),
(N'Константинова',N'Альбина', N'Леонидовна','19980219'),
(N'Шилова',N'Фаиза', N'Ростиславовна','19980219'),
(N'Овчинникова',N'Варвара', N'Леонидовна','19980219'),
(N'Маркова',N'Богдана', N'Куприяновна','19770219'),
(N'Самсонова',N'Эрида', N'Улебовна','19770219'),
(N'Лапина',N'Лика', N'Пантелеймоновна','19770219'),
(N'Гришина',N'Милослава', N'Михаиловна','19770219'),
(N'Крылова',N'Аэлита', N'Демьяновна','19590219'),
(N'Воронцова',N'Аксинья', N'Павловна','19590219'),
(N'Шарова',N'Златослава', N'Игнатьевна','19590219'),
(N'Кошелева',N'Агния', N'Георгьевна','19930219'),
(N'Князева',N'Октябрина', N'Матвеевна','19930219')


go
CREATE PROCEDURE UpdateEmployee
	@id INT,
    @lastName NVARCHAR(30),
    @firstName NVARCHAR(30),
	@middleName NVARCHAR(30),
    @birthday DATE
AS

 UPDATE dbo.Employees 
 SET
 [LastName] = @lastName,
 [FirstName] = @firstName,
 [MiddleName] = @middleName,
 [Birthday] = @birthday
 WHERE ID = @id
 
go
CREATE PROCEDURE InsertEmployee
    @lastName NVARCHAR(30),
    @firstName NVARCHAR(30),
	@middleName NVARCHAR(30),
    @birthday DATE,
	@ID int OUTPUT
AS
BEGIN
SET NOCOUNT ON;
INSERT Employees(LastName,FirstName, MiddleName, Birthday) VALUES
(@lastName,@firstName, @middleName,@birthday)
Set  @ID = SCOPE_IDENTITY()
END

go
CREATE PROCEDURE FindByFIO
    @lastName NVARCHAR(30),
    @firstName NVARCHAR(30),
	@middleName NVARCHAR(30)
AS
SELECT TOP 10  * 
FROM Employees
WHERE (UPPER(LastName) LIKE UPPER(@lastName +'%')) and (UPPER(FirstName) LIKE UPPER(@firstName+'%'))and (UPPER(MiddleName) LIKE UPPER(@middleName+'%'))

go
CREATE PROCEDURE GETEmployeesPage
   @pageSize INT,
   @endElement INT
AS
WITH num_row
AS(
  SELECT row_number() OVER (ORDER BY ID) as nom, ID
  FROM Employees
)

SELECT emp.* 
FROM (
	SELECT ID FROM num_row
	WHERE nom BETWEEN (@endElement - @pageSize) AND @endElement
) as foundElements
JOIN Employees as emp ON (foundElements.ID=emp.ID)

