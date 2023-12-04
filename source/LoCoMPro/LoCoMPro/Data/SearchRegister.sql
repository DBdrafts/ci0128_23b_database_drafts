CREATE VIEW dbo.SearchRegister WITH SCHEMABINDING AS
SELECT 
    r.SubmitionDate,
    r.ContributorId,
    r.ProductName,
    p.Brand,
    p.Model,
    r.StoreName,
    r.Price,
    c.ProvinciaName,
    c.CantonName,
    CASE
        WHEN s.Geolocation IS NOT NULL THEN s.Geolocation
        ELSE c.Geolocation
    END AS Geolocation
FROM dbo.Product p  -- Specify the schema for the Product table
    INNER JOIN dbo.Register r ON p.Name = r.ProductName
    INNER JOIN dbo.Canton c ON c.ProvinciaName = r.ProvinciaName AND c.CantonName = r.CantonName
    LEFT JOIN dbo.Store s ON s.Name = r.StoreName
    LEFT JOIN dbo.Report rp ON 
        rp.ContributorId = r.ContributorId AND
        rp.ProductName = r.ProductName AND
        rp.StoreName = r.StoreName AND
        rp.CantonName = r.CantonName AND
        rp.ProvinceName = r.ProvinciaName AND
        rp.SubmitionDate = r.SubmitionDate
WHERE rp.ReportState IS NULL OR rp.ReportState <> 2;
