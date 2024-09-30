CREATE TABLE Branch (
   _id VARCHAR(50) PRIMARY KEY,
   Name VARCHAR(100) NOT NULL,
   Created DATETIME DEFAULT CURRENT_TIMESTAMP,
   Modified DATETIME,
   Status BIT DEFAULT 1
);

CREATE TABLE Cashier(
    _id VARCHAR(50) PRIMARY KEY,
    Username VARCHAR(100) UNIQUE NOT NULL,
    Password VARCHAR(255) NOT NULL,
    FullName VARCHAR(100) NOT NULL,
    Admin BIT DEFAULT 0,
    BranchId VARCHAR(50),
	LastSeen DATETIME,
	Created DATETIME DEFAULT CURRENT_TIMESTAMP,
    Modified DATETIME,
    Status BIT DEFAULT 1,
    FOREIGN KEY (BranchId) REFERENCES Branch(_id)
);


CREATE TABLE Patient (
    _id VARCHAR(50) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Document VARCHAR(20) UNIQUE NOT NULL, 
    PhoneNumber1 VARCHAR(15),
    PhoneNumber2 VARCHAR(15),
    Address VARCHAR(255),
	Created DATETIME DEFAULT CURRENT_TIMESTAMP,
    Modified DATETIME,
    Status BIT DEFAULT 1
);

CREATE TABLE MedicalService (
    _id VARCHAR(50) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Description VARCHAR(255),
    Price DECIMAL(10, 2) NOT NULL,
	Created DATETIME DEFAULT CURRENT_TIMESTAMP,
    Modified DATETIME,
    Status BIT DEFAULT 1
);

CREATE TABLE TransactionType (
    _id INT PRIMARY KEY,
    Value VARCHAR(50) NOT NULL
);

CREATE TABLE "Transaction" (
    _id VARCHAR(50) PRIMARY KEY,
    CashierId VARCHAR(50),
    PatientId VARCHAR(50),
    ServiceId VARCHAR(50),
    TransactionTypeId INT NOT NULL,
    Amount DECIMAL(10, 2) NOT NULL,
    Date DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (CashierId) REFERENCES Cashier(_id),
    FOREIGN KEY (PatientId) REFERENCES Patient(_id),
    FOREIGN KEY (ServiceId) REFERENCES MedicalService(_id),
    FOREIGN KEY (TransactionTypeId) REFERENCES TransactionType(_id)
);


CREATE TABLE CashRegisterStatus (
    _id INT PRIMARY KEY,
    Value VARCHAR(50) NOT NULL
);


CREATE TABLE CashRegister (
    _id VARCHAR(50) PRIMARY KEY,
    CashierId VARCHAR(50),
    OpeningDate DATE,
    InitialAmount DECIMAL(10, 2) NOT NULL,
    CashInflow DECIMAL(10, 2) DEFAULT 0.00,
    CashOutflow DECIMAL(10, 2) DEFAULT 0.00,
    FinalAmount DECIMAL(10, 2) DEFAULT 0.00,
    CashRegisterStatusId INT,
    FOREIGN KEY (CashierId) REFERENCES Cashier(_id),
    FOREIGN KEY (CashRegisterStatusId) REFERENCES CashRegisterStatus(_id)
);

CREATE TABLE Receipt (
     _id VARCHAR(50) PRIMARY KEY,
    TransactionId VARCHAR(50),
    GenerationDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    Details VARCHAR(255), 
    FOREIGN KEY (TransactionId) REFERENCES "Transaction"(_id)
);


INSERT INTO Cashier (_id, Username, Password, Fullname)
VALUES ('8e7ba79d-3cde-46e7-b07e-44d7c25b4f33','jaime', '1234', 'Jaime Hernandez')