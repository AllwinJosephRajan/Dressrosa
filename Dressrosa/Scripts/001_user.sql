CREATE TABLE users (
    Id VARCHAR(255) PRIMARY KEY,
    FirstName VARCHAR(45),
    LastName VARCHAR(45),
    UserName VARCHAR(45),
    EmailAddress  VARCHAR(45),
    Password VARCHAR(512),
    PhoneNumber VARCHAR(45), 
    CreatedOn  DATETIME,
    UpdateOn DATETIME,
    CreatedBy VARCHAR(45),
    DeleteBit  BOOLEAN DEFAULT 0
);

INSERT INTO users (id, FirstName, LastName, UserName, EmailAddress, Password, PhoneNumber, CreatedOn, UpdateOn, CreatedBy)
VALUES (
    '1', 
    'Admin',
    'admin',
    'admin123',
    'admin@gmail.com',
    'dnaqr7AnyCW9mrq3iyNAcOcCdS9iW3UuVeVbSOYH41g=', -- admin@123
    '123-456-7890',
    NOW(),
    NOW(),
    'Admnin'
);
