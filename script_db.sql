-- Create Database
CREATE DATABASE SportStoreDB2;
GO

-- Use the created database
USE SportStoreDB2;
GO

-- Create Tables
CREATE TABLE Users (
                       UserID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                       Username NVARCHAR(50) NOT NULL,
                       Password NVARCHAR(100) NOT NULL,
                       Email NVARCHAR(100) NOT NULL,
                       FullName NVARCHAR(100) NOT NULL,
                       PhoneNumber NVARCHAR(20) NULL,
                       Role NVARCHAR(50) NOT NULL,
                       Address NVARCHAR(MAX) NOT NULL,
                       CreatedAt DATETIME NULL
);

CREATE TABLE Categories (
                            CategoryID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                            Name NVARCHAR(100) NOT NULL,
                            Description NVARCHAR(MAX) NULL
);

CREATE TABLE Products (
                          ProductID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                          Name NVARCHAR(100) NOT NULL,
                          Description NVARCHAR(MAX) NULL,
                          Price DECIMAL(18, 2) NOT NULL,
                          StockQuantity INT NOT NULL,
                          CategoryID INT NOT NULL,
                          Color VARCHAR(50) NULL,
                          Size VARCHAR(MAX) NULL,
    Specifications NVARCHAR(MAX) NULL,
    CONSTRAINT FK_Product_Category FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID) ON DELETE CASCADE
);

CREATE TABLE Blog (
                      BlogId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                      Title NVARCHAR(255) NOT NULL,
                      Content NVARCHAR(MAX) NOT NULL,
                      Author NVARCHAR(100) NOT NULL,
                      CreatedAt DATETIME NOT NULL,
                      ImageUrl NVARCHAR(255) NULL,
                      CategoryId INT NULL,
                      UserId INT NULL,
                      Tags NVARCHAR(255) NULL,
                      CONSTRAINT FK_Blog_Category FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryID),
                      CONSTRAINT FK_Blog_User FOREIGN KEY (UserId) REFERENCES Users(UserID)
);

CREATE TABLE Cart (
                      CartID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                      UserID INT NOT NULL,
                      ProductID INT NOT NULL,
                      Quantity INT NOT NULL,
                      CONSTRAINT FK_Cart_User FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
                      CONSTRAINT FK_Cart_Product FOREIGN KEY (ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE
);

CREATE TABLE Comment (
                         CommentId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                         Content NVARCHAR(255) NOT NULL,
                         CreatedAt DATETIME NOT NULL,
                         ProductId INT NOT NULL,
                         UserId INT NOT NULL,
                         CONSTRAINT FK_Comment_Product FOREIGN KEY (ProductId) REFERENCES Products(ProductID),
                         CONSTRAINT FK_Comment_User FOREIGN KEY (UserId) REFERENCES Users(UserID)
);

CREATE TABLE Images (
                        ImageID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        ProductID INT NOT NULL,
                        ImagePath NVARCHAR(MAX) NULL,
                        CONSTRAINT FK_Image_Product FOREIGN KEY (ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE
);

CREATE TABLE Orders (
                        OrderID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        UserID INT NOT NULL,
                        OrderDate DATETIME NOT NULL,
                        TotalAmount DECIMAL(18, 2) NOT NULL,
                        OrderStatus NVARCHAR(50) NOT NULL,
                        ShippingMethod NVARCHAR(255) NULL,
                        PaymentMethod NVARCHAR(255) NULL,
                        ShippingAddress NVARCHAR(1000) NULL,
                        CONSTRAINT FK_Order_User FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
);

CREATE TABLE OrderDetails (
                              OrderDetailID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                              OrderID INT NOT NULL,
                              ProductID INT NOT NULL,
                              Quantity INT NOT NULL,
                              UnitPrice DECIMAL(18, 2) NOT NULL,
                              CONSTRAINT FK_OrderDetails_Order FOREIGN KEY (OrderID) REFERENCES Orders(OrderID) ON DELETE CASCADE,
                              CONSTRAINT FK_OrderDetails_Product FOREIGN KEY (ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE
);

CREATE TABLE Rating (
                        RatingId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        Value INT NOT NULL,
                        CreatedAt DATETIME NOT NULL,
                        ProductId INT NOT NULL,
                        UserId INT NOT NULL,
                        CONSTRAINT FK_Rating_Product FOREIGN KEY (ProductId) REFERENCES Products(ProductID),
                        CONSTRAINT FK_Rating_User FOREIGN KEY (UserId) REFERENCES Users(UserID)
);

CREATE TABLE Shipment (
                          ShipmentID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                          UserID INT NOT NULL,
                          ShipmentAddress NVARCHAR(MAX) NOT NULL,
                          ShipmentDate DATETIME NULL,
                          ShipmentStatus NVARCHAR(50) NULL,
                          CONSTRAINT FK_Shipment_User FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
);

CREATE TABLE Specification (
                               SpecificationId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                               Name VARCHAR(100) NOT NULL,
                               Value NVARCHAR(MAX) NOT NULL,
                               ProductId INT NOT NULL,
                               CONSTRAINT FK_Specification_Product FOREIGN KEY (ProductId) REFERENCES Products(ProductID)
);

CREATE TABLE Wishlist (
                          WishlistID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                          UserID INT NOT NULL,
                          ProductID INT NOT NULL,
                          CONSTRAINT FK_Wishlist_User FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
                          CONSTRAINT FK_Wishlist_Product FOREIGN KEY (ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE
);

CREATE TABLE __EFMigrationsHistory (
                                       MigrationId NVARCHAR(150) NOT NULL PRIMARY KEY,
                                       ProductVersion NVARCHAR(32) NOT NULL
);
GO