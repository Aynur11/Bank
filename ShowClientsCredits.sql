SELECT
Clients.Id,
Clients.Vip,
Clients.Name,
Clients.Sum,
Clients.DepositPercent,
Clients.CreditPercent,
Credits.Rate as 'CreditRate',
Credits.Period as 'CreditPeriod',
Credits.Sum as 'CreditSum',
Deposits.Capitalization  as 'DepositCapitalization',
Deposits.Rate  as 'DepositRate',
Deposits.Period as 'DepositPeriod',
Deposits.Sum  as 'DepositSum'
FROM Clients, Credits, Deposits
WHERE Clients.PhysicalPersonsDepartment='True' and Clients.Id = Credits.ClientId and Clients.Id = Deposits.ClientId