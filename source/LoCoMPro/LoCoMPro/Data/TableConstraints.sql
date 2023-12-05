ALTER TABLE IMAGE
DROP CONSTRAINT FK_Image_Register_ContributorId_ProductName_StoreName_CantonName_ProvinceName_SubmitionDate;

ALTER TABLE Report
DROP CONSTRAINT FK_Report_Register_ContributorId_ProductName_StoreName_CantonName_ProvinceName_SubmitionDate;

ALTER TABLE Review
DROP CONSTRAINT FK_Review_Register_ContributorId_ProductName_StoreName_CantonName_ProvinceName_SubmitionDate;

ALTER TABLE IMAGE
ADD CONSTRAINT FK_IMAGE_REGISTER
FOREIGN KEY ([ContributorId], [ProductName], [StoreName], [CantonName], [ProvinceName], [SubmitionDate]) REFERENCES [dbo].[Register] ([ContributorId], [ProductName], [StoreName], [CantonName], [ProvinciaName], [SubmitionDate])
ON DELETE NO ACTION
ON UPDATE CASCADE;

ALTER TABLE Report
ADD CONSTRAINT FK_REPORT_REGISTER
FOREIGN KEY ([ContributorId], [ProductName], [StoreName], [CantonName], [ProvinceName], [SubmitionDate]) REFERENCES [dbo].[Register] ([ContributorId], [ProductName], [StoreName], [CantonName], [ProvinciaName], [SubmitionDate])
ON DELETE NO ACTION
ON UPDATE CASCADE;

ALTER TABLE Review
ADD CONSTRAINT FK_REVIEW_REGISTER
FOREIGN KEY ([ContributorId], [ProductName], [StoreName], [CantonName], [ProvinceName], [SubmitionDate]) REFERENCES [dbo].[Register] ([ContributorId], [ProductName], [StoreName], [CantonName], [ProvinciaName], [SubmitionDate])
ON DELETE NO ACTION
ON UPDATE CASCADE;