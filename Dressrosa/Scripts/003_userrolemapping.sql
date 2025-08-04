CREATE TABLE user_rolemapping (
	Id VARCHAR(255) PRIMARY KEY,
    UserId VARCHAR(255),
    RoleId VARCHAR(255),
    FOREIGN KEY (UserId) REFERENCES users(Id),
    FOREIGN KEY (RoleId) REFERENCES roles(Id)
);
INSERT INTO user_rolemapping (Id, UserId, RoleId)
VALUES 
    ('01', '1', '1');
