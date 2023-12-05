CREATE  NONCLUSTERED INDEX IX_Review_ContributorId
ON REVIEW ([ContributorId]) INCLUDE ([ReviewValue]);