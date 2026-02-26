<?php
header("Content-Type: application/json");

$mysqli = new mysqli("localhost", "root", "", "db_MKON");

if ($mysqli->connect_errno) {
    echo json_encode(["ok" => false, "error" => "DB connection failed"]);
    exit;
}

$mapName = $_GET["mapName"] ?? "";

$stmt = $mysqli->prepare("SELECT RaceTime, RoundTime FROM besttimes WHERE Mapname = ?");
$stmt->bind_param("s", $mapName);
$stmt->execute();
$stmt->store_result();

if ($stmt->num_rows === 0) {
    echo json_encode(["ok" => false, "error" => "Map not found"]);
    exit;
}

$stmt->bind_result($raceTime, $roundTime);
$stmt->fetch();

echo json_encode([
    "ok" => true,
    "raceTime" => floatval($raceTime),
    "roundTime" => floatval($roundTime)
]);
