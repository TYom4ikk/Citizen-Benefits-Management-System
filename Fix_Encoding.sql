USE [BenefitsManagementSystem]
GO

-- Исправление данных пользователя admin
UPDATE [dbo].[Users]
SET 
    [FirstName] = N'Администратор',
    [LastName] = N'Системы'
WHERE [UserID] = 1;

-- Исправление категорий льгот
UPDATE [dbo].[BenefitCategories]
SET 
    [CategoryName] = N'Ветеран труда',
    [Description] = N'Льготы ветеранов труда',
    [LegalBasis] = N'ФЗ "О ветеранах"'
WHERE [CategoryID] = 1;

UPDATE [dbo].[BenefitCategories]
SET 
    [CategoryName] = N'Ветеран боевых действий',
    [Description] = N'Льготы ветеранов боевых действий',
    [LegalBasis] = N'ФЗ "О ветеранах"'
WHERE [CategoryID] = 2;

UPDATE [dbo].[BenefitCategories]
SET 
    [CategoryName] = N'Инвалид I группы',
    [Description] = N'Льготы инвалидов I группы',
    [LegalBasis] = N'ФЗ "О социальной защите инвалидов"'
WHERE [CategoryID] = 3;

UPDATE [dbo].[BenefitCategories]
SET 
    [CategoryName] = N'Инвалид II группы',
    [Description] = N'Льготы инвалидов II группы',
    [LegalBasis] = N'ФЗ "О социальной защите инвалидов"'
WHERE [CategoryID] = 4;

UPDATE [dbo].[BenefitCategories]
SET 
    [CategoryName] = N'Многодетная семья',
    [Description] = N'Льготы многодетных семей',
    [LegalBasis] = N'Указ Президента РФ № 431'
WHERE [CategoryID] = 5;

UPDATE [dbo].[BenefitCategories]
SET 
    [CategoryName] = N'Чернобылец',
    [Description] = N'Льготы чернобыльцев',
    [LegalBasis] = N'ФЗ "О дополнительных гарантиях чернобыльцев"'
WHERE [CategoryID] = 6;

-- Исправление типов документов
UPDATE [dbo].[DocumentTypes]
SET 
    [DocTypeName] = N'Паспорт гражданина РФ',
    [Description] = N'Основной документ, удостоверяющий личность гражданина РФ'
WHERE [DocTypeID] = 1;

UPDATE [dbo].[DocumentTypes]
SET 
    [DocTypeName] = N'СНИЛС',
    [Description] = N'Страховой номер индивидуального лицевого счета'
WHERE [DocTypeID] = 2;

UPDATE [dbo].[DocumentTypes]
SET 
    [DocTypeName] = N'ИНН',
    [Description] = N'Идентификационный номер налогоплательщика'
WHERE [DocTypeID] = 3;

UPDATE [dbo].[DocumentTypes]
SET 
    [DocTypeName] = N'Пенсионное удостоверение',
    [Description] = N'Удостоверение пенсионного удостоверения'
WHERE [DocTypeID] = 4;

-- Исправление регионов
UPDATE [dbo].[Regions]
SET [RegionName] = N'Москва'
WHERE [RegionID] = 1;

UPDATE [dbo].[Regions]
SET [RegionName] = N'Санкт-Петербург'
WHERE [RegionID] = 2;

UPDATE [dbo].[Regions]
SET [RegionName] = N'Московская область'
WHERE [RegionID] = 3;

UPDATE [dbo].[Regions]
SET [RegionName] = N'Свердловская область'
WHERE [RegionID] = 4;

UPDATE [dbo].[Regions]
SET [RegionName] = N'Республика Татарстан'
WHERE [RegionID] = 5;

-- Исправление ролей пользователей
UPDATE [dbo].[UserRoles]
SET 
    [RoleName] = N'Администратор',
    [Description] = N'Полный доступ к системе'
WHERE [RoleID] = 1;

UPDATE [dbo].[UserRoles]
SET 
    [RoleName] = N'Оператор',
    [Description] = N'Доступ к основным функциям системы'
WHERE [RoleID] = 2;

UPDATE [dbo].[UserRoles]
SET 
    [RoleName] = N'Гражданин',
    [Description] = N'Доступ к личным данным'
WHERE [RoleID] = 3;

GO

PRINT 'Кодировка данных исправлена успешно!'
