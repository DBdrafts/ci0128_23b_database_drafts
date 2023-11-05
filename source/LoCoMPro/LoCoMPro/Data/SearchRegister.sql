CREATE VIEW SearchRegister AS
SELECT r.SubmitionDate, r.ContributorId, r.ProductName, p.Brand, p.Model,
	r.StoreName, r.Price, c.ProvinciaName, c.CantonName, c.Geolocation
	
FROM Product p INNER JOIN Register r ON p.Name = r.ProductName
			INNER JOIN Canton c ON c.ProvinciaName = r.ProvinciaName AND
								c.CantonName = r.CantonName
			LEFT JOIN Report rp ON rp.ContributorId = r.ContributorId AND
                  rp.ProductName = r.ProductName AND
                  rp.StoreName = r.StoreName AND
                  rp.CantonName = r.CantonName AND
                  rp.ProvinceName = r.ProvinciaName AND 
                  rp.SubmitionDate = r.SubmitionDate AND 
                  rp.ReportState != 2