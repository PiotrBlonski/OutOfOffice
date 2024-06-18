-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jun 18, 2024 at 06:35 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.0.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `outofofficedb`
--

-- --------------------------------------------------------

--
-- Table structure for table `approvalrequest`
--

CREATE TABLE `approvalrequest` (
  `Id` int(11) NOT NULL,
  `Approver_Id` int(11) NOT NULL,
  `LeaveRequest_Id` int(11) NOT NULL,
  `Status` int(11) NOT NULL,
  `Comment` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

-- --------------------------------------------------------

--
-- Table structure for table `employee`
--

CREATE TABLE `employee` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) NOT NULL,
  `Subdivision_Id` int(11) NOT NULL,
  `Position_Id` int(11) NOT NULL,
  `Status` int(11) NOT NULL,
  `Balance` int(11) NOT NULL,
  `Login` varchar(50) NOT NULL,
  `Password` varchar(1024) NOT NULL,
  `RefreshToken` varchar(1024) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Dumping data for table `employee`
--

INSERT INTO `employee` (`Id`, `Name`, `Subdivision_Id`, `Position_Id`, `Status`, `Balance`, `Login`, `Password`, `RefreshToken`) VALUES
(1, 'Leo Balik', 1, 1, 1, 2376, 'leo1', '$2b$10$Tvyt.mwe2WgfXU0T7lkzMOu005txInPT7CSroArrN/aZHvU.764Ty', 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoibGVvMSIsImlhdCI6MTcxODcxOTc1NH0.pKktJ1ys3Sbu28OjzuhnJruiqFZZcJAUzC8q3aN1XdU'),
(2, 'Daniel Głowa', 4, 1, 1, 240, 'daniel1', '$2b$10$DBQYcYHM/Nl7caTd44qnXeqPE8fLXVDNDD25rNv.R1.JpDZ1drv8q', NULL),
(3, 'Igor Kamrowski', 1, 1, 1, 240, 'igor1', '$2b$10$40NZcyxsIaOczV/sYo8Zx.mvRTQw9dtsJqVvSkaV9yHK6NpSwDwDC', NULL),
(4, 'Halina Pawlicka', 2, 1, 1, 240, 'halina1', '$2b$10$QVp2cp7eSiBvq2yuvrTvkeclu5a/drOg1j0S46sOawmH4AH.pETQa', NULL),
(5, 'Irena Jarocka', 5, 1, 1, 240, 'irena1', '$2b$10$nL34sHSwwwJZ8PveQ1IYqOIWa9.feb9d9uPn8cpajWAPQDdEzKEXG', NULL),
(6, 'Alina Giertych', 3, 1, 1, 240, 'alina1', '$2b$10$qDseja4e5QoMwz1ZeuVrcuApBKorEDh44ak3p4VJQ0tJAQJn9ihW.', NULL),
(7, 'Izabella Tuszyńska', 2, 2, 1, 240, 'izabella1', '$2b$10$KZfZiA5vkH5kLePeXCFcCu5u5eRHEqGaA7KDqmETVz9ywyoWA/OnO', 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiaXphYmVsbGExIiwiaWF0IjoxNzE4NzIyMzA3fQ.CaMUEleVcdWTKb_9MllOJC6DXNZ2wFC9Y287JBRIPDc'),
(8, 'Janusz Frąk', 3, 2, 1, 240, 'janusz1', '$2b$10$X4Rb2.IIyU4N1tIpYJxl1u1FxcnTaCwMAzVz9qhBBt6dX23vFtbry', 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiamFudXN6MSIsImlhdCI6MTcxODcxNjU0OX0.a0JKw07FVeXXK9YX4Clc0pbOl9YBL0B0SKyEjAMuNyQ'),
(9, 'Maciej Sośnicki', 4, 3, 1, 240, 'maciej11', '$2b$10$Zc/bINEHVt54A61XHXQmk.7LnNj5FHTKjh.EE2.yY2QwEAGKr9UEK', NULL),
(10, 'Kuba Miłoś', 4, 3, 1, 240, 'kuba1', '$2b$10$CfHfP0oabFqqWPU6hSXLyeXA8u0JH/fBdFrM99e51pBWxm2dFU/ZW', 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoia3ViYTEiLCJpYXQiOjE3MTg3MjQ1MzR9.whPJtckypPDvKpqWSzbT55m8JtaTRUB3jSM20D9IVv8'),
(11, 'Stanisław Goral', 1, 4, 1, 240, 'stanislaw1', '$2b$10$TjW9T7xet6NuRW2fGodbhuQODM4bXaAkvPKuI09Ob1lR5sTxyJyNy', 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoic3RhbmlzbGF3MSIsImlhdCI6MTcxODcyNzc4M30.LXduOnDIXtxeF_8e_OduRyGMngWZYXhZyQu41cPFI08'),
(12, 'Anna Nowak', 4, 1, 1, 240, 'anna1', '$2b$10$lQ7UbHrmeGGx7cR7w6Tr2e76m5H/QvXYlKUzE8j28NnG1nlfIO8K2', NULL),
(13, 'Filip Kowalski', 3, 1, 2, 240, 'filip1', '$2b$10$l3f06JTQv.nPcB6mKIWYReWdKr4GEV7b97mBxsR1io/NqwBy15.F6', NULL),
(14, 'Weronika Pałąk', 3, 1, 1, 240, 'weronika1', '$2b$10$YJ2/AoZvWkNmDY7FAcwXxezJN4NQg2jAcWwNE9KAqYiVZ0U2M/9UK', NULL),
(15, 'Wojciech Kot', 3, 1, 1, 240, 'wojciech1', '$2b$10$wx2RD/1vHhZQGrNxGW.8aOga7afBASEQBDq.KCEWzgaoYnYvTtDwC', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `employeeposition`
--

CREATE TABLE `employeeposition` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Dumping data for table `employeeposition`
--

INSERT INTO `employeeposition` (`Id`, `Name`) VALUES
(1, 'Employee'),
(2, 'HR Manager'),
(3, 'Project Manager'),
(4, 'Administrator');

-- --------------------------------------------------------

--
-- Table structure for table `leaverequest`
--

CREATE TABLE `leaverequest` (
  `Id` int(11) NOT NULL,
  `Employee_Id` int(11) NOT NULL,
  `Reason_Id` int(11) NOT NULL,
  `StartDate` date NOT NULL,
  `EndDate` date NOT NULL,
  `Comment` text DEFAULT NULL,
  `Status` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Dumping data for table `leaverequest`
--

INSERT INTO `leaverequest` (`Id`, `Employee_Id`, `Reason_Id`, `StartDate`, `EndDate`, `Comment`, `Status`) VALUES
(7, 1, 4, '2024-06-19', '2024-06-21', NULL, 1);

-- --------------------------------------------------------

--
-- Table structure for table `peoplepartner`
--

CREATE TABLE `peoplepartner` (
  `Id` int(11) NOT NULL,
  `Employee_Id` int(11) NOT NULL,
  `Partner_Id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Dumping data for table `peoplepartner`
--

INSERT INTO `peoplepartner` (`Id`, `Employee_Id`, `Partner_Id`) VALUES
(3, 3, 7),
(5, 5, 7),
(6, 6, 8),
(21, 1, 7),
(22, 2, 8),
(23, 4, 8),
(26, 15, 7);

-- --------------------------------------------------------

--
-- Table structure for table `project`
--

CREATE TABLE `project` (
  `Name` varchar(50) NOT NULL,
  `Id` int(11) NOT NULL,
  `ProjectType_Id` int(11) NOT NULL,
  `StartDate` date NOT NULL,
  `EndDate` date DEFAULT NULL,
  `Manager_Id` int(11) NOT NULL,
  `Comment` text DEFAULT NULL,
  `Status` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Dumping data for table `project`
--

INSERT INTO `project` (`Name`, `Id`, `ProjectType_Id`, `StartDate`, `EndDate`, `Manager_Id`, `Comment`, `Status`) VALUES
('Project 2', 2, 12, '2024-07-18', '2024-08-10', 10, '', 1);

-- --------------------------------------------------------

--
-- Table structure for table `projectmember`
--

CREATE TABLE `projectmember` (
  `Id` int(11) NOT NULL,
  `Project_Id` int(11) NOT NULL,
  `Employee_Id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Dumping data for table `projectmember`
--

INSERT INTO `projectmember` (`Id`, `Project_Id`, `Employee_Id`) VALUES
(4, 2, 4),
(5, 2, 5),
(6, 2, 6),
(7, 2, 1);

-- --------------------------------------------------------

--
-- Table structure for table `projecttype`
--

CREATE TABLE `projecttype` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Dumping data for table `projecttype`
--

INSERT INTO `projecttype` (`Id`, `Name`) VALUES
(1, 'Business implementation'),
(2, 'Foundational (business improvement)'),
(3, 'IT infrastructure improvement'),
(4, 'IT infrastructure improvement'),
(5, 'Product development (IT)'),
(6, 'Product development (non-IT)'),
(7, 'Physical engineering/construction'),
(8, 'Physical infrastructure improvement'),
(9, 'Procurement'),
(10, 'Regulatory/compliance'),
(11, 'Research and Development (R&D)'),
(12, 'Service development'),
(13, 'Transformation/reengineering'),
(14, 'Other');

-- --------------------------------------------------------

--
-- Table structure for table `reason`
--

CREATE TABLE `reason` (
  `Id` int(11) NOT NULL,
  `Name` varchar(256) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Dumping data for table `reason`
--

INSERT INTO `reason` (`Id`, `Name`) VALUES
(1, 'Accident'),
(2, 'Death of family member'),
(3, 'Personal Illness'),
(4, 'Child’s Illness'),
(5, 'Emergency'),
(6, 'Car Problem'),
(7, 'Medical Appointment');

-- --------------------------------------------------------

--
-- Table structure for table `subdivision`
--

CREATE TABLE `subdivision` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_polish_ci;

--
-- Dumping data for table `subdivision`
--

INSERT INTO `subdivision` (`Id`, `Name`) VALUES
(1, 'IT'),
(2, 'HR'),
(3, 'Marketing'),
(4, 'Accounting'),
(5, 'Office');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `approvalrequest`
--
ALTER TABLE `approvalrequest`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `Approver_Id` (`Approver_Id`),
  ADD KEY `LeaveRequest_Id` (`LeaveRequest_Id`);

--
-- Indexes for table `employee`
--
ALTER TABLE `employee`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `Position_Id` (`Position_Id`),
  ADD KEY `Subdivision_Id` (`Subdivision_Id`);

--
-- Indexes for table `employeeposition`
--
ALTER TABLE `employeeposition`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `leaverequest`
--
ALTER TABLE `leaverequest`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `Employee_Id` (`Employee_Id`),
  ADD KEY `Reason_Id` (`Reason_Id`);

--
-- Indexes for table `peoplepartner`
--
ALTER TABLE `peoplepartner`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `Employee_Id` (`Employee_Id`),
  ADD KEY `Partner_Id` (`Partner_Id`);

--
-- Indexes for table `project`
--
ALTER TABLE `project`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `Manager_Id` (`Manager_Id`),
  ADD KEY `ProjectType_Id` (`ProjectType_Id`);

--
-- Indexes for table `projectmember`
--
ALTER TABLE `projectmember`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `Project_Id` (`Project_Id`),
  ADD KEY `Employee_Id` (`Employee_Id`);

--
-- Indexes for table `projecttype`
--
ALTER TABLE `projecttype`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `reason`
--
ALTER TABLE `reason`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `subdivision`
--
ALTER TABLE `subdivision`
  ADD PRIMARY KEY (`Id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `approvalrequest`
--
ALTER TABLE `approvalrequest`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `employee`
--
ALTER TABLE `employee`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT for table `employeeposition`
--
ALTER TABLE `employeeposition`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `leaverequest`
--
ALTER TABLE `leaverequest`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT for table `peoplepartner`
--
ALTER TABLE `peoplepartner`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=27;

--
-- AUTO_INCREMENT for table `project`
--
ALTER TABLE `project`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `projectmember`
--
ALTER TABLE `projectmember`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT for table `projecttype`
--
ALTER TABLE `projecttype`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

--
-- AUTO_INCREMENT for table `reason`
--
ALTER TABLE `reason`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT for table `subdivision`
--
ALTER TABLE `subdivision`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `approvalrequest`
--
ALTER TABLE `approvalrequest`
  ADD CONSTRAINT `approvalrequest_ibfk_1` FOREIGN KEY (`Approver_Id`) REFERENCES `employee` (`Id`),
  ADD CONSTRAINT `approvalrequest_ibfk_2` FOREIGN KEY (`LeaveRequest_Id`) REFERENCES `leaverequest` (`Id`);

--
-- Constraints for table `employee`
--
ALTER TABLE `employee`
  ADD CONSTRAINT `employee_ibfk_1` FOREIGN KEY (`Position_Id`) REFERENCES `employeeposition` (`Id`),
  ADD CONSTRAINT `employee_ibfk_2` FOREIGN KEY (`Subdivision_Id`) REFERENCES `subdivision` (`Id`);

--
-- Constraints for table `leaverequest`
--
ALTER TABLE `leaverequest`
  ADD CONSTRAINT `leaverequest_ibfk_1` FOREIGN KEY (`Employee_Id`) REFERENCES `employee` (`Id`),
  ADD CONSTRAINT `leaverequest_ibfk_2` FOREIGN KEY (`Reason_Id`) REFERENCES `reason` (`Id`);

--
-- Constraints for table `peoplepartner`
--
ALTER TABLE `peoplepartner`
  ADD CONSTRAINT `peoplepartner_ibfk_1` FOREIGN KEY (`Employee_Id`) REFERENCES `employee` (`Id`),
  ADD CONSTRAINT `peoplepartner_ibfk_2` FOREIGN KEY (`Partner_Id`) REFERENCES `employee` (`Id`);

--
-- Constraints for table `project`
--
ALTER TABLE `project`
  ADD CONSTRAINT `project_ibfk_1` FOREIGN KEY (`Manager_Id`) REFERENCES `employee` (`Id`),
  ADD CONSTRAINT `project_ibfk_2` FOREIGN KEY (`ProjectType_Id`) REFERENCES `projecttype` (`Id`);

--
-- Constraints for table `projectmember`
--
ALTER TABLE `projectmember`
  ADD CONSTRAINT `projectmember_ibfk_1` FOREIGN KEY (`Project_Id`) REFERENCES `project` (`Id`),
  ADD CONSTRAINT `projectmember_ibfk_2` FOREIGN KEY (`Employee_Id`) REFERENCES `employee` (`Id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
