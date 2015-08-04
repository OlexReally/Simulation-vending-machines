-- --------------------------------------------------------
-- Сервер:                       127.0.0.1
-- Версія сервера:               5.5.23 - MySQL Community Server (GPL)
-- ОС сервера:                   Win64
-- HeidiSQL Версія:              9.2.0.4947
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- Dumping database structure for simulate
CREATE DATABASE IF NOT EXISTS `simulate` /*!40100 DEFAULT CHARACTER SET cp1251 COLLATE cp1251_ukrainian_ci */;
USE `simulate`;


-- Dumping structure for таблиця simulate.automats
CREATE TABLE IF NOT EXISTS `automats` (
  `id` int(11) DEFAULT NULL,
  `name` char(50) COLLATE cp1251_ukrainian_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=cp1251 COLLATE=cp1251_ukrainian_ci;

-- Dumping data for table simulate.automats: ~0 rows (приблизно)
/*!40000 ALTER TABLE `automats` DISABLE KEYS */;
/*!40000 ALTER TABLE `automats` ENABLE KEYS */;


-- Dumping structure for таблиця simulate.result
CREATE TABLE IF NOT EXISTS `result` (
  `id` int(11) DEFAULT NULL COMMENT 'id автоматів',
  `time` datetime DEFAULT NULL,
  `type` int(11) DEFAULT NULL,
  `suma` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=cp1251 COLLATE=cp1251_ukrainian_ci;

-- Dumping data for table simulate.result: ~0 rows (приблизно)
/*!40000 ALTER TABLE `result` DISABLE KEYS */;
/*!40000 ALTER TABLE `result` ENABLE KEYS */;


-- Dumping structure for таблиця simulate.transactions
CREATE TABLE IF NOT EXISTS `transactions` (
  `id` int(11) DEFAULT NULL,
  `name` char(50) COLLATE cp1251_ukrainian_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=cp1251 COLLATE=cp1251_ukrainian_ci;

-- Dumping data for table simulate.transactions: ~3 rows (приблизно)
/*!40000 ALTER TABLE `transactions` DISABLE KEYS */;
INSERT INTO `transactions` (`id`, `name`) VALUES
	(1, 'Чай'),
	(2, 'Кава'),
	(3, 'Вода');
/*!40000 ALTER TABLE `transactions` ENABLE KEYS */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
