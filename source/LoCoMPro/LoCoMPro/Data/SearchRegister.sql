CREATE VIEW SearchRegister AS
SELECT r.SubmitionDate, r.ContributorId, r.ProductName, p.Brand, p.Model,
	r.StoreName, r.Price, r.NumCorrections, r.Comment, c.ProvinciaName,
	c.CantonName, c.Geolocation
FROM Product p INNER JOIN Register r ON p.Name = r.ProductName
			INNER JOIN Canton c ON c.ProvinciaName = r.ProvinciaName AND
								c.CantonName = r.CantonName;