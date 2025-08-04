 CREATE TABLE roles (
    Id VARCHAR(100) PRIMARY KEY,
    RoleName VARCHAR(100) NOT NULL,
    CreatedOn  datetime NULL
);
INSERT INTO roles (Id, RoleName, CreatedOn )
VALUES ('1','Admin', '2025-08-17 00:00:00');
