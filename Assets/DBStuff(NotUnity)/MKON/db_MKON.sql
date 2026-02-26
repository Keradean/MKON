-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Erstellungszeit: 19. Feb 2026 um 15:16
-- Server-Version: 10.4.32-MariaDB
-- PHP-Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Datenbank: `db_mkon`
--

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `besttimes`
--

CREATE TABLE `besttimes` (
  `Mapname` varchar(32) NOT NULL,
  `RaceTime` float NOT NULL,
  `RoundTime` float NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- Daten für Tabelle `besttimes`
--

INSERT INTO `besttimes` (`Mapname`, `RaceTime`, `RoundTime`) VALUES
('DesertCity', 510.43, 150.43),
('LevelDevil', 100000000, 100000000),
('Snowland', 100000000, 100000000);

--
-- Indizes der exportierten Tabellen
--

--
-- Indizes für die Tabelle `besttimes`
--
ALTER TABLE `besttimes`
  ADD PRIMARY KEY (`Mapname`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
