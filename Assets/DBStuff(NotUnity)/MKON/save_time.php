<?php
header("Content-Type: application/json");

$mysqli = new mysqli("localhost", "root", "", "db_MKON");

if ($mysqli->connect_errno) {
    echo json_encode(["ok" => false, "error" => "DB connection failed"]);
    exit;
}

$data = json_decode(file_get_contents("php://input"), true);

$mapName = $data["mapName"] ?? "";
$time = $data["time"] ?? null;
$column = $data["column"] ?? "";

// Sicherheitscheck: Nur erlaubte Spalten
if (!in_array($column, ["RaceTime", "RoundTime"])) {
    echo json_encode(["ok" => false, "error" => "Invalid column"]);
    exit;
}

$stmt = $mysqli->prepare("UPDATE besttimes SET $column = ? WHERE Mapname = ?");
$stmt->bind_param("ds", $time, $mapName);
$stmt->execute();

if ($stmt->affected_rows === 0) {
    echo json_encode(["ok" => false, "error" => "Map not found or no change"]);
    exit;
}

echo json_encode(["ok" => true]);
